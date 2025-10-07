using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAccessLogRepository accessLogRepository,
        IJwtTokenService jwtTokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessLogRepository = accessLogRepository;
        _jwtTokenService = jwtTokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _refreshTokenRepository.FindByTokenAsync(request.RefreshToken, cancellationToken)
                                      ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!existingRefreshToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token is no longer active.");
        }

        var user = await _userRepository.FindByIdAsync(existingRefreshToken.UserId, cancellationToken)
                   ?? throw new UnauthorizedAccessException("User not found for refresh token.");

        existingRefreshToken.RevokedAt = _dateTimeProvider.UtcNow;
        await _refreshTokenRepository.UpdateAsync(existingRefreshToken, cancellationToken);

        var tokenResult = _jwtTokenService.GenerateTokens(user);
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = tokenResult.RefreshToken,
            CreatedAt = _dateTimeProvider.UtcNow,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        await _accessLogRepository.AddAsync(new AccessLog
        {
            UserId = user.Id,
            Action = "TOKEN_REFRESH",
            IpAddress = request.IpAddress,
            OccurredAt = _dateTimeProvider.UtcNow
        }, cancellationToken);

        return new AuthResultDto(
            user.Id,
            user.Email,
            user.Role,
            tokenResult.AccessToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt);
    }
}
