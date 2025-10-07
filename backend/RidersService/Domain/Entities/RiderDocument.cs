using RidersService.Domain.Enums;

namespace RidersService.Domain.Entities;

public class RiderDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderId { get; set; }
    public DocumentType Type { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsVerified { get; set; }
    public Rider? Rider { get; set; }
}
