namespace ReportsService.Domain.Models;

public sealed record RevenueSnapshot(
    decimal TotalGrossRevenue,
    decimal TotalPlatformRevenue,
    int TotalCompletedOrders
);
