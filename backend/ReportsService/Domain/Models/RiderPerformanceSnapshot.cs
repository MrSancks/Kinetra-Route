namespace ReportsService.Domain.Models;

public sealed record RiderPerformanceSnapshot(
    string RiderId,
    int CompletedOrders,
    int OnTimeDeliveries,
    double? AverageDeliveryDurationMinutes,
    decimal TotalRevenueGenerated
)
{
    public double OnTimeRate => CompletedOrders == 0 ? 0 : (double)OnTimeDeliveries / CompletedOrders;
}
