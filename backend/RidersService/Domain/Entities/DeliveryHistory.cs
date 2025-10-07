namespace RidersService.Domain.Entities;

public class DeliveryHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public Rider? Rider { get; set; }
}
