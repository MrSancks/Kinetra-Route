using AuthService.Application.DTOs;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password, string? IpAddress) : IRequest<AuthResultDto>;
