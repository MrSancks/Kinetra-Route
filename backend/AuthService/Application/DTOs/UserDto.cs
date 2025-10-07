using AuthService.Domain.Enums;

namespace AuthService.Application.DTOs;

public record UserDto(Guid Id, string Email, Role Role, string? FullName, DateTime CreatedAt, DateTime? LastLoginAt);
