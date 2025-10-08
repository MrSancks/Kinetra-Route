namespace PaymentsService.Contracts.Requests;

public sealed class CalculateTariffRequest
{
    public DeliveryOrderRequest Order { get; set; } = new();
}
