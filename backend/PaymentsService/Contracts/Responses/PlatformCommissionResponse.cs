namespace PaymentsService.Contracts.Responses;

public sealed record PlatformCommissionResponse(
    decimal TotalPlatformCommission,
    decimal TotalGrossEarnings
);
