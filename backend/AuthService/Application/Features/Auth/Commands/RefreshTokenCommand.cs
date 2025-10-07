using AuthService.Application.DTOs;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<AuthResultDto>;
