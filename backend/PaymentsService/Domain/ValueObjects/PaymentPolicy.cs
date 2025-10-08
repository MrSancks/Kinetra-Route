namespace PaymentsService.Domain.ValueObjects;

public sealed record PaymentPolicy(
    Money BaseFee,
    Money PerKilometerRate,
    Percentage PlatformCommissionRate,
    Distance LongDistanceThreshold,
    Money LongDistanceBonus)
{
    public static PaymentPolicy Create(
        decimal baseFee,
        decimal perKilometerRate,
        decimal platformCommissionRate,
        decimal longDistanceThresholdKm,
        decimal longDistanceBonus)
    {
        return new PaymentPolicy(
            Money.From(baseFee),
            Money.From(perKilometerRate),
            Percentage.FromFraction(platformCommissionRate),
            Distance.FromKilometers(longDistanceThresholdKm),
            Money.From(longDistanceBonus));
    }
}
