namespace PaymentsService.Contracts.Requests;

public sealed class RealTimeReportRequest
{
    public List<DeliveryOrderRequest> Orders { get; set; } = new();
}
