using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Domain.Entities;

public sealed record OrderPaymentBreakdown(
    string OrderId,
    Money GrossTotal,
    Money PlatformCommission,
    Money RiderPayout,
    bool LongDistanceBonusApplied
);
