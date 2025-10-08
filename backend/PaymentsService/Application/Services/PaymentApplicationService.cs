using PaymentsService.Application.Abstractions;
using PaymentsService.Contracts.Requests;
using PaymentsService.Contracts.Responses;
using PaymentsService.Domain.Entities;
using PaymentsService.Domain.Services;
using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Application.Services;

public sealed class PaymentApplicationService
{
    private readonly PaymentDomainService _domainService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PaymentApplicationService(PaymentDomainService domainService, IDateTimeProvider dateTimeProvider)
    {
        _domainService = domainService;
        _dateTimeProvider = dateTimeProvider;
    }

    public OrderTariffResponse CalculateOrderTariff(CalculateTariffRequest request)
    {
        var order = MapOrder(request.Order);
        var breakdown = _domainService.CalculateOrderPayment(order);

        return new OrderTariffResponse(
            breakdown.OrderId,
            breakdown.GrossTotal.ToDecimal(),
            breakdown.RiderPayout.ToDecimal(),
            breakdown.PlatformCommission.ToDecimal(),
            breakdown.LongDistanceBonusApplied);
    }

    public WeeklySettlementResponse GenerateWeeklySettlement(WeeklySettlementRequest request)
    {
        var rider = Rider.Create(request.RiderId, request.RiderName);
        var period = DateRange.Create(DateOnly.FromDateTime(request.WeekStart), DateOnly.FromDateTime(request.WeekEnd));
        var orders = request.Orders.Select(MapOrder).ToList();

        var settlement = _domainService.CalculateWeeklySettlement(rider, period, orders, _dateTimeProvider.UtcNow);

        return new WeeklySettlementResponse(
            settlement.RiderId,
            settlement.RiderName,
            settlement.Period.Start,
            settlement.Period.End,
            settlement.OrdersCount,
            settlement.GrossEarnings.ToDecimal(),
            settlement.PlatformCommission.ToDecimal(),
            settlement.NetEarnings.ToDecimal(),
            settlement.GeneratedAt);
    }

    public PlatformCommissionResponse CalculatePlatformCommission(PlatformCommissionRequest request)
    {
        var orders = request.Orders.Select(MapOrder).ToList();
        var commission = _domainService.CalculatePlatformCommission(orders);

        var totalGross = Money.Zero;
        foreach (var order in orders)
        {
            var breakdown = _domainService.CalculateOrderPayment(order);
            totalGross = totalGross.Add(breakdown.GrossTotal);
        }

        return new PlatformCommissionResponse(
            commission.ToDecimal(),
            totalGross.ToDecimal());
    }

    public RealTimeReportResponse GenerateRealTimeReport(RealTimeReportRequest request)
    {
        var orders = request.Orders.Select(MapOrder).ToList();
        var report = _domainService.GenerateRealTimeReport(orders, _dateTimeProvider.UtcNow);

        return new RealTimeReportResponse(
            report.TotalGross.ToDecimal(),
            report.TotalRiderPayout.ToDecimal(),
            report.TotalPlatformCommission.ToDecimal(),
            report.ActiveRiders,
            report.GeneratedAt);
    }

    private static DeliveryOrder MapOrder(DeliveryOrderRequest request)
    {
        var distance = Distance.FromKilometers(request.DistanceInKilometers);
        var baseFee = Money.From(request.BaseFee);
        return DeliveryOrder.Create(request.OrderId, request.RiderId, distance, baseFee, request.CompletedAt);
    }
}
