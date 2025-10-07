using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAccessLogRepository accessLogRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessLogRepository = accessLogRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            await _accessLogRepository.AddAsync(new AccessLog
            {
                UserId = user.Id,
                Action = "LOGIN_FAILED",
                IpAddress = request.IpAddress,
                OccurredAt = _dateTimeProvider.UtcNow
            }, cancellationToken);

            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var now = _dateTimeProvider.UtcNow;
        user.LastLoginAt = now;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var tokenResult = _jwtTokenService.GenerateTokens(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = tokenResult.RefreshToken,
            CreatedAt = now,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _accessLogRepository.AddAsync(new AccessLog
        {
            UserId = user.Id,
            Action = "LOGIN_SUCCESS",
            IpAddress = request.IpAddress,
            OccurredAt = now
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
