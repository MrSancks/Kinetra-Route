using System.Globalization;
using ReportsService.Domain.Enums;
using ReportsService.Domain.Events;
using ReportsService.Domain.Models;

namespace ReportsService.Application.MaterializedViews;

public sealed class ReportMaterializedViewStore
{
    private readonly object _sync = new();
    private readonly Dictionary<DateOnly, OrderPeriodAccumulator> _ordersByDay = new();
    private readonly Dictionary<WeekKey, OrderPeriodAccumulator> _ordersByWeek = new();
    private readonly Dictionary<MonthKey, OrderPeriodAccumulator> _ordersByMonth = new();
    private readonly Dictionary<string, RiderPerformanceAccumulator> _riderPerformance = new();

    public void Apply(OrderCompletedEvent @event)
    {
        var completionDate = DateOnly.FromDateTime(@event.CompletedAt);
        var weekKey = new WeekKey(@event.CompletedAt.Year, ISOWeek.GetWeekOfYear(@event.CompletedAt));
        var monthKey = new MonthKey(@event.CompletedAt.Year, @event.CompletedAt.Month);

        lock (_sync)
        {
            UpdateAccumulator(_ordersByDay, completionDate, @event.OrderTotal, @event.PlatformFee);
            UpdateAccumulator(_ordersByWeek, weekKey, @event.OrderTotal, @event.PlatformFee);
            UpdateAccumulator(_ordersByMonth, monthKey, @event.OrderTotal, @event.PlatformFee);
            UpdateRiderAccumulator(@event);
        }
    }

    public IReadOnlyList<OrderPeriodSummary> GetOrdersSummaries(PeriodGranularity granularity, DateOnly? start, DateOnly? end)
    {
        lock (_sync)
        {
            return granularity switch
            {
                PeriodGranularity.Daily => ProjectDaySummaries(start, end),
                PeriodGranularity.Weekly => ProjectWeekSummaries(start, end),
                PeriodGranularity.Monthly => ProjectMonthSummaries(start, end),
                _ => throw new ArgumentOutOfRangeException(nameof(granularity), granularity, null)
            };
        }
    }

    public IReadOnlyList<RiderPerformanceSnapshot> GetRiderPerformance(string? riderId)
    {
        lock (_sync)
        {
            IEnumerable<KeyValuePair<string, RiderPerformanceAccumulator>> source = _riderPerformance;

            if (!string.IsNullOrWhiteSpace(riderId))
            {
                if (_riderPerformance.TryGetValue(riderId, out var accumulator))
                {
                    source = new[] { new KeyValuePair<string, RiderPerformanceAccumulator>(riderId, accumulator) };
                }
                else
                {
                    source = Array.Empty<KeyValuePair<string, RiderPerformanceAccumulator>>();
                }
            }

            return source
                .Select(pair => pair.Value.ToSnapshot(pair.Key))
                .OrderByDescending(snapshot => snapshot.CompletedOrders)
                .ThenBy(snapshot => snapshot.RiderId, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    public RevenueSnapshot GetRevenueSnapshot(DateOnly? start, DateOnly? end)
    {
        lock (_sync)
        {
            var applicableDays = FilterByRange(_ordersByDay, start, end);

            var totalGross = applicableDays.Sum(entry => entry.Value.GrossRevenue);
            var totalPlatform = applicableDays.Sum(entry => entry.Value.PlatformRevenue);
            var totalOrders = applicableDays.Sum(entry => entry.Value.Orders);

            return new RevenueSnapshot(totalGross, totalPlatform, totalOrders);
        }
    }

    private IReadOnlyList<OrderPeriodSummary> ProjectDaySummaries(DateOnly? start, DateOnly? end)
    {
        return FilterByRange(_ordersByDay, start, end)
            .OrderBy(entry => entry.Key)
            .Select(entry => entry.Value.ToSummary(
                PeriodGranularity.Daily,
                entry.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                entry.Key,
                entry.Key))
            .ToList();
    }

    private IReadOnlyList<OrderPeriodSummary> ProjectWeekSummaries(DateOnly? start, DateOnly? end)
    {
        var entries = FilterByRange(_ordersByWeek, start, end)
            .OrderBy(entry => entry.Key.Year)
            .ThenBy(entry => entry.Key.Week);

        var summaries = new List<OrderPeriodSummary>();

        foreach (var entry in entries)
        {
            var (year, week) = entry.Key;
            var weekStart = DateOnly.FromDateTime(ISOWeek.ToDateTime(year, week, DayOfWeek.Monday));
            var weekEnd = weekStart.AddDays(6);

            summaries.Add(entry.Value.ToSummary(
                PeriodGranularity.Weekly,
                $"{year}-W{week:D2}",
                weekStart,
                weekEnd));
        }

        return summaries;
    }

    private IReadOnlyList<OrderPeriodSummary> ProjectMonthSummaries(DateOnly? start, DateOnly? end)
    {
        var entries = FilterByRange(_ordersByMonth, start, end)
            .OrderBy(entry => entry.Key.Year)
            .ThenBy(entry => entry.Key.Month);

        var summaries = new List<OrderPeriodSummary>();

        foreach (var entry in entries)
        {
            var (year, month) = entry.Key;
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            summaries.Add(entry.Value.ToSummary(
                PeriodGranularity.Monthly,
                $"{year}-{month:D2}",
                monthStart,
                monthEnd));
        }

        return summaries;
    }

    private static void UpdateAccumulator<TKey>(Dictionary<TKey, OrderPeriodAccumulator> source, TKey key, decimal orderTotal, decimal platformFee)
        where TKey : notnull
    {
        if (!source.TryGetValue(key, out var accumulator))
        {
            accumulator = new OrderPeriodAccumulator();
            source[key] = accumulator;
        }

        accumulator.Register(orderTotal, platformFee);
    }

    private void UpdateRiderAccumulator(OrderCompletedEvent @event)
    {
        if (!_riderPerformance.TryGetValue(@event.RiderId, out var accumulator))
        {
            accumulator = new RiderPerformanceAccumulator();
            _riderPerformance[@event.RiderId] = accumulator;
        }

        accumulator.Register(@event);
    }

    private static IEnumerable<KeyValuePair<TKey, OrderPeriodAccumulator>> FilterByRange<TKey>(
        Dictionary<TKey, OrderPeriodAccumulator> source,
        DateOnly? start,
        DateOnly? end)
        where TKey : notnull
    {
        if (start is null && end is null)
        {
            return source.ToArray();
        }

        return source.Where(entry => IsWithinRange(entry.Key, start, end)).ToArray();
    }

    private static bool IsWithinRange<TKey>(TKey key, DateOnly? start, DateOnly? end)
    {
        return key switch
        {
            DateOnly day => (start is null || day >= start) && (end is null || day <= end),
            WeekKey weekKey => IsWeekWithinRange(weekKey, start, end),
            MonthKey monthKey => IsMonthWithinRange(monthKey, start, end),
            _ => true
        };
    }

    private static bool IsWeekWithinRange(WeekKey key, DateOnly? start, DateOnly? end)
    {
        if (start is null && end is null)
        {
            return true;
        }

        var startDate = DateOnly.FromDateTime(ISOWeek.ToDateTime(key.Year, key.Week, DayOfWeek.Monday));
        var endDate = startDate.AddDays(6);

        return (start is null || endDate >= start) && (end is null || startDate <= end);
    }

    private static bool IsMonthWithinRange(MonthKey key, DateOnly? start, DateOnly? end)
    {
        if (start is null && end is null)
        {
            return true;
        }

        var startDate = new DateOnly(key.Year, key.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return (start is null || endDate >= start) && (end is null || startDate <= end);
    }

    private sealed class OrderPeriodAccumulator
    {
        public int Orders { get; private set; }
        public decimal GrossRevenue { get; private set; }
        public decimal PlatformRevenue { get; private set; }

        public void Register(decimal orderTotal, decimal platformFee)
        {
            Orders++;
            GrossRevenue += orderTotal;
            PlatformRevenue += platformFee;
        }

        public OrderPeriodSummary ToSummary(PeriodGranularity granularity, string periodKey, DateOnly start, DateOnly end)
        {
            return new OrderPeriodSummary(granularity, periodKey, start, end, Orders, GrossRevenue, PlatformRevenue);
        }
    }

    private sealed class RiderPerformanceAccumulator
    {
        private int _completedOrders;
        private int _onTimeOrders;
        private double _totalDuration;
        private int _durationSamples;
        private decimal _totalRevenue;

        public void Register(OrderCompletedEvent @event)
        {
            _completedOrders++;
            if (@event.DeliveredOnTime)
            {
                _onTimeOrders++;
            }

            if (@event.DeliveryDurationMinutes is { } duration)
            {
                _totalDuration += duration;
                _durationSamples++;
            }

            _totalRevenue += @event.OrderTotal;
        }

        public RiderPerformanceSnapshot ToSnapshot(string riderId)
        {
            double? averageDuration = _durationSamples == 0 ? null : _totalDuration / _durationSamples;

            return new RiderPerformanceSnapshot(
                riderId,
                _completedOrders,
                _onTimeOrders,
                averageDuration,
                _totalRevenue);
        }
    }

    private readonly record struct WeekKey(int Year, int Week)
    {
        public void Deconstruct(out int year, out int week)
        {
            year = Year;
            week = Week;
        }
    }

    private readonly record struct MonthKey(int Year, int Month)
    {
        public void Deconstruct(out int year, out int month)
        {
            year = Year;
            month = Month;
        }
    }
}
