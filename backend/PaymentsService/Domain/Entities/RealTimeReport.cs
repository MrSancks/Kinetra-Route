using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Domain.Entities;

public sealed class RealTimeReport
{
    public Money TotalGross { get; }
    public Money TotalRiderPayout { get; }
    public Money TotalPlatformCommission { get; }
    public int ActiveRiders { get; }
    public DateTime GeneratedAt { get; }

    private RealTimeReport(Money totalGross, Money totalRiderPayout, Money totalPlatformCommission, int activeRiders, DateTime generatedAt)
    {
        TotalGross = totalGross;
        TotalRiderPayout = totalRiderPayout;
        TotalPlatformCommission = totalPlatformCommission;
        ActiveRiders = activeRiders;
        GeneratedAt = generatedAt;
    }

    public static RealTimeReport Create(Money totalGross, Money totalRiderPayout, Money totalPlatformCommission, int activeRiders, DateTime generatedAt)
    {
        if (activeRiders < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(activeRiders), "Active riders cannot be negative.");
        }

        return new RealTimeReport(totalGross, totalRiderPayout, totalPlatformCommission, activeRiders, generatedAt);
    }
}
