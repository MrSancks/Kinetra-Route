using ReportsService.Domain.Enums;

namespace ReportsService.Domain.Models;

public sealed record OrderPeriodSummary(
    PeriodGranularity Granularity,
    string PeriodKey,
    DateOnly RangeStart,
    DateOnly RangeEnd,
    int Orders,
    decimal GrossRevenue,
    decimal PlatformRevenue
);
