namespace PaymentsService.Contracts.Responses;

public sealed record OrderTariffResponse(
    string OrderId,
    decimal Total,
    decimal RiderPayout,
    decimal PlatformCommission,
    bool LongDistanceBonusApplied
);
