namespace ReportsService.Contracts.Responses;

public sealed record RevenueBreakdownDto(
    string Period,
    DateOnly RangeStart,
    DateOnly RangeEnd,
    decimal GrossRevenue,
    decimal PlatformRevenue,
    int Orders
);

public sealed record RevenueAnalysisResponse(
    DateOnly? Start,
    DateOnly? End,
    decimal TotalGrossRevenue,
    decimal TotalPlatformRevenue,
    int TotalCompletedOrders,
    IReadOnlyList<RevenueBreakdownDto> Breakdown
);
