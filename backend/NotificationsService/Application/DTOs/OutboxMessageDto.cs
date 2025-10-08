namespace NotificationsService.Application.DTOs;

public record OutboxMessageDto(
    Guid Id,
    string EventType,
    string Payload,
    DateTimeOffset OccurredOn,
    DateTimeOffset? ProcessedOn,
    string? Error
);
