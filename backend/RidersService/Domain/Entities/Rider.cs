using RidersService.Domain.Enums;

namespace RidersService.Domain.Entities;

public class Rider
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Offline;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Vehicle? Vehicle { get; set; }
    public ICollection<RiderDocument> Documents { get; set; } = new List<RiderDocument>();
    public ICollection<DeliveryHistory> Deliveries { get; set; } = new List<DeliveryHistory>();
}
