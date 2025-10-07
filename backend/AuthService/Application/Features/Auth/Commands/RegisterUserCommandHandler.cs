using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Features.Auth.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterUserCommandHandler(
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

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException($"The email '{request.Email}' is already registered.");
        }

        var now = _dateTimeProvider.UtcNow;
        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName,
            Role = request.Role,
            CreatedAt = now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);

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
            Action = BuildActionName(request.Role),
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

    private static string BuildActionName(Role role) => role switch
    {
        Role.Rider => "REGISTER_RIDER",
        Role.Restaurant => "REGISTER_RESTAURANT",
        Role.Admin => "REGISTER_ADMIN",
        _ => "REGISTER_UNKNOWN"
    };
}
