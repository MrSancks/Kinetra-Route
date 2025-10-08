namespace PaymentsService.Contracts.Requests;

public sealed class PlatformCommissionRequest
{
    public List<DeliveryOrderRequest> Orders { get; set; } = new();
}
