namespace ReportsService.Domain.Events;

public sealed record OrderCompletedEvent(
    string OrderId,
    string RiderId,
    DateTime CompletedAt,
    decimal OrderTotal,
    decimal PlatformFee,
    bool DeliveredOnTime,
    double? DeliveryDurationMinutes
);
