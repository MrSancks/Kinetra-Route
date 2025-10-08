using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Domain.Entities;

public sealed class RiderSettlement
{
    public string RiderId { get; }
    public string RiderName { get; }
    public DateRange Period { get; }
    public int OrdersCount { get; }
    public Money GrossEarnings { get; }
    public Money PlatformCommission { get; }
    public Money NetEarnings { get; }
    public DateTime GeneratedAt { get; }

    private RiderSettlement(
        string riderId,
        string riderName,
        DateRange period,
        int ordersCount,
        Money grossEarnings,
        Money platformCommission,
        Money netEarnings,
        DateTime generatedAt)
    {
        RiderId = riderId;
        RiderName = riderName;
        Period = period;
        OrdersCount = ordersCount;
        GrossEarnings = grossEarnings;
        PlatformCommission = platformCommission;
        NetEarnings = netEarnings;
        GeneratedAt = generatedAt;
    }

    public static RiderSettlement Create(
        string riderId,
        string riderName,
        DateRange period,
        int ordersCount,
        Money grossEarnings,
        Money platformCommission,
        Money netEarnings,
        DateTime generatedAt)
    {
        if (string.IsNullOrWhiteSpace(riderId))
        {
            throw new ArgumentException("Rider identifier cannot be empty.", nameof(riderId));
        }

        if (string.IsNullOrWhiteSpace(riderName))
        {
            throw new ArgumentException("Rider name cannot be empty.", nameof(riderName));
        }

        if (ordersCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordersCount), "Orders count cannot be negative.");
        }

        return new RiderSettlement(riderId, riderName, period, ordersCount, grossEarnings, platformCommission, netEarnings, generatedAt);
    }
}
