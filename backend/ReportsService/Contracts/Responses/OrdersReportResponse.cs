using ReportsService.Domain.Enums;

namespace ReportsService.Contracts.Responses;

public sealed record OrderPeriodSummaryDto(
    string Period,
    DateOnly RangeStart,
    DateOnly RangeEnd,
    int Orders,
    decimal GrossRevenue,
    decimal PlatformRevenue
);

public sealed record OrdersReportResponse(
    PeriodGranularity Granularity,
    DateOnly? Start,
    DateOnly? End,
    IReadOnlyList<OrderPeriodSummaryDto> Periods
);
