using NotificationsService.Domain.Enums;

namespace NotificationsService.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RecipientId { get; set; } = string.Empty;
    public RecipientType RecipientType { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public int DeliveryAttempts { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SentAt { get; set; }
}
