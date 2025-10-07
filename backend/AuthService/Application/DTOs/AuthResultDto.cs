using AuthService.Domain.Enums;

namespace AuthService.Application.DTOs;

public record AuthResultDto(
    Guid UserId,
    string Email,
    Role Role,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
