using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions;

public record TokenResult(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt);

public interface IJwtTokenService
{
    TokenResult GenerateTokens(User user);
    Guid? GetUserIdFromExpiredToken(string token);
}
