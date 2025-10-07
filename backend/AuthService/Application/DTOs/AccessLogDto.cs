using AuthService.Domain.Enums;

namespace AuthService.Application.DTOs;

public record AccessLogDto(Guid Id, Guid UserId, string Email, Role Role, string Action, DateTime OccurredAt, string? IpAddress);
