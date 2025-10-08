namespace PaymentsService.Contracts.Requests;

public sealed class WeeklySettlementRequest
{
    public string RiderId { get; set; } = string.Empty;
    public string RiderName { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public List<DeliveryOrderRequest> Orders { get; set; } = new();
}
