namespace PaymentsService.Contracts.Responses;

public sealed record RealTimeReportResponse(
    decimal TotalGross,
    decimal TotalRiderPayout,
    decimal TotalPlatformCommission,
    int ActiveRiders,
    DateTime GeneratedAtUtc
);
