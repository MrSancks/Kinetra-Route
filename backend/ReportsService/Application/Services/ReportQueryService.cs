using ReportsService.Application.MaterializedViews;
using ReportsService.Contracts.Responses;
using ReportsService.Domain.Enums;

namespace ReportsService.Application.Services;

public sealed class ReportQueryService
{
    private readonly ReportMaterializedViewStore _viewStore;

    public ReportQueryService(ReportMaterializedViewStore viewStore)
    {
        _viewStore = viewStore;
    }

    public ValueTask<OrdersReportResponse> GetOrdersReportAsync(PeriodGranularity granularity, DateOnly? start, DateOnly? end, CancellationToken cancellationToken = default)
    {
        var summaries = _viewStore.GetOrdersSummaries(granularity, start, end)
            .Select(summary => new OrderPeriodSummaryDto(
                summary.PeriodKey,
                summary.RangeStart,
                summary.RangeEnd,
                summary.Orders,
                summary.GrossRevenue,
                summary.PlatformRevenue))
            .ToList();

        var response = new OrdersReportResponse(granularity, start, end, summaries);
        return ValueTask.FromResult(response);
    }

    public ValueTask<RiderPerformanceResponse> GetRiderPerformanceAsync(string? riderId, CancellationToken cancellationToken = default)
    {
        var riders = _viewStore.GetRiderPerformance(riderId)
            .Select(snapshot => new RiderPerformanceMetricDto(
                snapshot.RiderId,
                snapshot.CompletedOrders,
                snapshot.OnTimeDeliveries,
                Math.Round(snapshot.OnTimeRate, 4, MidpointRounding.AwayFromZero),
                snapshot.AverageDeliveryDurationMinutes,
                snapshot.TotalRevenueGenerated))
            .ToList();

        return ValueTask.FromResult(new RiderPerformanceResponse(riders));
    }

    public ValueTask<RevenueAnalysisResponse> GetRevenueAnalysisAsync(PeriodGranularity granularity, DateOnly? start, DateOnly? end, CancellationToken cancellationToken = default)
    {
        var snapshot = _viewStore.GetRevenueSnapshot(start, end);
        var breakdown = _viewStore.GetOrdersSummaries(granularity, start, end)
            .Select(summary => new RevenueBreakdownDto(
                summary.PeriodKey,
                summary.RangeStart,
                summary.RangeEnd,
                summary.GrossRevenue,
                summary.PlatformRevenue,
                summary.Orders))
            .ToList();

        var response = new RevenueAnalysisResponse(
            start,
            end,
            snapshot.TotalGrossRevenue,
            snapshot.TotalPlatformRevenue,
            snapshot.TotalCompletedOrders,
            breakdown);

        return ValueTask.FromResult(response);
    }
}
