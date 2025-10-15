using System.Text.Json;
using PaymentsService.Application.Abstractions;
using PaymentsService.Contracts.Requests;
using PaymentsService.Contracts.Responses;
using PaymentsService.Domain.Entities;
using PaymentsService.Domain.Services;
using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Application.Services;

public sealed class PaymentApplicationService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly PaymentDomainService _domainService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPaymentComputationRepository _computationRepository;

    public PaymentApplicationService(
        PaymentDomainService domainService,
        IDateTimeProvider dateTimeProvider,
        IPaymentComputationRepository computationRepository)
    {
        _domainService = domainService;
        _dateTimeProvider = dateTimeProvider;
        _computationRepository = computationRepository;
    }

    public async Task<OrderTariffResponse> CalculateOrderTariffAsync(
        CalculateTariffRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = MapOrder(request.Order);
        var breakdown = _domainService.CalculateOrderPayment(order);

        var response = new OrderTariffResponse(
            breakdown.OrderId,
            breakdown.GrossTotal.ToDecimal(),
            breakdown.RiderPayout.ToDecimal(),
            breakdown.PlatformCommission.ToDecimal(),
            breakdown.LongDistanceBonusApplied);

        await PersistComputationAsync("OrderTariff", request, response, cancellationToken).ConfigureAwait(false);

        return response;
    }

    public async Task<WeeklySettlementResponse> GenerateWeeklySettlementAsync(
        WeeklySettlementRequest request,
        CancellationToken cancellationToken = default)
    {
        var rider = Rider.Create(request.RiderId, request.RiderName);
        var period = DateRange.Create(DateOnly.FromDateTime(request.WeekStart), DateOnly.FromDateTime(request.WeekEnd));
        var orders = request.Orders.Select(MapOrder).ToList();

        var settlement = _domainService.CalculateWeeklySettlement(rider, period, orders, _dateTimeProvider.UtcNow);

        var response = new WeeklySettlementResponse(
            settlement.RiderId,
            settlement.RiderName,
            settlement.Period.Start,
            settlement.Period.End,
            settlement.OrdersCount,
            settlement.GrossEarnings.ToDecimal(),
            settlement.PlatformCommission.ToDecimal(),
            settlement.NetEarnings.ToDecimal(),
            settlement.GeneratedAt);

        await PersistComputationAsync("WeeklySettlement", request, response, cancellationToken).ConfigureAwait(false);

        return response;
    }

    public async Task<PlatformCommissionResponse> CalculatePlatformCommissionAsync(
        PlatformCommissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var orders = request.Orders.Select(MapOrder).ToList();
        var commission = _domainService.CalculatePlatformCommission(orders);

        var totalGross = Money.Zero;
        foreach (var order in orders)
        {
            var breakdown = _domainService.CalculateOrderPayment(order);
            totalGross = totalGross.Add(breakdown.GrossTotal);
        }

        var response = new PlatformCommissionResponse(
            commission.ToDecimal(),
            totalGross.ToDecimal());

        await PersistComputationAsync("PlatformCommission", request, response, cancellationToken).ConfigureAwait(false);

        return response;
    }

    public async Task<RealTimeReportResponse> GenerateRealTimeReportAsync(
        RealTimeReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var orders = request.Orders.Select(MapOrder).ToList();
        var report = _domainService.GenerateRealTimeReport(orders, _dateTimeProvider.UtcNow);

        var response = new RealTimeReportResponse(
            report.TotalGross.ToDecimal(),
            report.TotalRiderPayout.ToDecimal(),
            report.TotalPlatformCommission.ToDecimal(),
            report.ActiveRiders,
            report.GeneratedAt);

        await PersistComputationAsync("RealTimeReport", request, response, cancellationToken).ConfigureAwait(false);

        return response;
    }

    private static DeliveryOrder MapOrder(DeliveryOrderRequest request)
    {
        var distance = Distance.FromKilometers(request.DistanceInKilometers);
        var baseFee = Money.From(request.BaseFee);
        return DeliveryOrder.Create(request.OrderId, request.RiderId, distance, baseFee, request.CompletedAt);
    }

    private Task PersistComputationAsync(string type, object request, object response, CancellationToken cancellationToken)
    {
        var requestPayload = JsonSerializer.Serialize(request, SerializerOptions);
        var responsePayload = JsonSerializer.Serialize(response, SerializerOptions);
        return _computationRepository.SaveAsync(type, requestPayload, responsePayload, cancellationToken);
    }
}
