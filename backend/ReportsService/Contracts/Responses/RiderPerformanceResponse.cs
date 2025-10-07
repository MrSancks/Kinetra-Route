namespace ReportsService.Contracts.Responses;

public sealed record RiderPerformanceMetricDto(
    string RiderId,
    int CompletedOrders,
    int OnTimeDeliveries,
    double OnTimeRate,
    double? AverageDeliveryDurationMinutes,
    decimal TotalRevenueGenerated
);

public sealed record RiderPerformanceResponse(
    IReadOnlyList<RiderPerformanceMetricDto> Riders
);
