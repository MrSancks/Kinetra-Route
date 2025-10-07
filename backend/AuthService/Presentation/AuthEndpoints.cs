using System.Net.Mime;
using AuthService.Application.DTOs;
using AuthService.Application.Features.AccessLogs.Queries;
using AuthService.Application.Features.Auth.Commands;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Presentation;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .Accepts<RegisterRequest>(MediaTypeNames.Application.Json)
            .Produces<AuthResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/register/rider", async (RegisterRequest request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FullName, Role.Rider, ResolveIp(context));
            return Results.Ok(await sender.Send(command, cancellationToken));
        });

        group.MapPost("/register/restaurant", async (RegisterRequest request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FullName, Role.Restaurant, ResolveIp(context));
            return Results.Ok(await sender.Send(command, cancellationToken));
        });

        group.MapPost("/register/admin", [Authorize(Roles = nameof(Role.Admin))] async (RegisterRequest request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FullName, Role.Admin, ResolveIp(context));
            return Results.Ok(await sender.Send(command, cancellationToken));
        });

        group.MapPost("/login", async (LoginRequest request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password, ResolveIp(context));
            return Results.Ok(await sender.Send(command, cancellationToken));
        });

        group.MapPost("/refresh", async (RefreshTokenRequest request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken, ResolveIp(context));
            return Results.Ok(await sender.Send(command, cancellationToken));
        });

        group.MapGet("/logs", [Authorize(Roles = nameof(Role.Admin))] async ([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? userId, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetAccessLogsQuery(from, to, userId);
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }

    private static string? ResolveIp(HttpContext context) => context.Connection.RemoteIpAddress?.ToString();

    public record RegisterRequest(string Email, string Password, string? FullName);
    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string RefreshToken);
}
