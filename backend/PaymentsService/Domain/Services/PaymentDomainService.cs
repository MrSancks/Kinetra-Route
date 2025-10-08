using PaymentsService.Domain.Entities;
using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Domain.Services;

public sealed class PaymentDomainService
{
    private readonly PaymentPolicy _policy;

    public PaymentDomainService(PaymentPolicy policy)
    {
        _policy = policy;
    }

    public OrderPaymentBreakdown CalculateOrderPayment(DeliveryOrder order)
    {
        var variableComponent = _policy.PerKilometerRate.Multiply(order.Distance.ToDecimal());
        var bonus = order.Distance.IsLongerThan(_policy.LongDistanceThreshold)
            ? _policy.LongDistanceBonus
            : Money.Zero;

        var gross = order.BaseFee.Add(variableComponent).Add(bonus);
        var commission = gross.Multiply(_policy.PlatformCommissionRate.Fraction);
        var riderPayout = gross.Subtract(commission);

        return new OrderPaymentBreakdown(
            order.OrderId,
            gross,
            commission,
            riderPayout,
            bonus != Money.Zero);
    }

    public RiderSettlement CalculateWeeklySettlement(Rider rider, DateRange period, IEnumerable<DeliveryOrder> orders, DateTime generatedAt)
    {
        var relevantOrders = orders
            .Where(order => order.RiderId == rider.RiderId && period.Contains(DateOnly.FromDateTime(order.CompletedAt)))
            .ToList();

        var gross = Money.Zero;
        var commission = Money.Zero;
        var riderPayout = Money.Zero;

        foreach (var order in relevantOrders)
        {
            var breakdown = CalculateOrderPayment(order);
            gross = gross.Add(breakdown.GrossTotal);
            commission = commission.Add(breakdown.PlatformCommission);
            riderPayout = riderPayout.Add(breakdown.RiderPayout);
        }

        return RiderSettlement.Create(
            rider.RiderId,
            rider.Name,
            period,
            relevantOrders.Count,
            gross,
            commission,
            riderPayout,
            generatedAt);
    }

    public Money CalculatePlatformCommission(IEnumerable<DeliveryOrder> orders)
    {
        var total = Money.Zero;
        foreach (var order in orders)
        {
            var breakdown = CalculateOrderPayment(order);
            total = total.Add(breakdown.PlatformCommission);
        }

        return total;
    }

    public RealTimeReport GenerateRealTimeReport(IEnumerable<DeliveryOrder> orders, DateTime generatedAt)
    {
        var orderList = orders.ToList();
        var totalGross = Money.Zero;
        var totalCommission = Money.Zero;
        var totalRiderPayout = Money.Zero;

        foreach (var order in orderList)
        {
            var breakdown = CalculateOrderPayment(order);
            totalGross = totalGross.Add(breakdown.GrossTotal);
            totalCommission = totalCommission.Add(breakdown.PlatformCommission);
            totalRiderPayout = totalRiderPayout.Add(breakdown.RiderPayout);
        }

        var activeRiders = orderList
            .Select(order => order.RiderId)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        return RealTimeReport.Create(totalGross, totalRiderPayout, totalCommission, activeRiders, generatedAt);
    }
}
