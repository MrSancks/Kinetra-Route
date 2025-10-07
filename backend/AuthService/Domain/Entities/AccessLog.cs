namespace AuthService.Domain.Entities;

public class AccessLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime OccurredAt { get; set; }

    public User? User { get; set; }
}
