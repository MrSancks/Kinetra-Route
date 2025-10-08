namespace PaymentsService.Contracts.Responses;

public sealed record WeeklySettlementResponse(
    string RiderId,
    string RiderName,
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int OrdersCount,
    decimal GrossEarnings,
    decimal PlatformCommission,
    decimal NetEarnings,
    DateTime GeneratedAtUtc
);
