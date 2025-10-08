namespace PaymentsService.Contracts.Requests;

public sealed class DeliveryOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string RiderId { get; set; } = string.Empty;
    public decimal DistanceInKilometers { get; set; }
    public decimal BaseFee { get; set; }
    public DateTime CompletedAt { get; set; }
}
