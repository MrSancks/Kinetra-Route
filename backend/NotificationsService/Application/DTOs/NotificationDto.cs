using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.DTOs;

public record NotificationDto(
    Guid Id,
    string RecipientId,
    RecipientType RecipientType,
    NotificationChannel Channel,
    NotificationType Type,
    string Title,
    string Message,
    NotificationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt
);
