using AuthService.Application.DTOs;
using AuthService.Domain.Enums;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands;

public record RegisterUserCommand(string Email, string Password, string? FullName, Role Role, string? IpAddress) : IRequest<AuthResultDto>;
