namespace PaymentsService.Infrastructure.Configuration;

public sealed class PaymentPolicyOptions
{
    public const string SectionName = "PaymentPolicy";

    public decimal BaseFee { get; set; } = 25m;
    public decimal PerKilometerRate { get; set; } = 5m;
    public decimal PlatformCommissionRate { get; set; } = 0.2m;
    public decimal LongDistanceThresholdKm { get; set; } = 6m;
    public decimal LongDistanceBonus { get; set; } = 10m;
}
