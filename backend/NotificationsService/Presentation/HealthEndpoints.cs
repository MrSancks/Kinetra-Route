using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace NotificationsService.Presentation;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "OK", service = "NotificationsService" }))
            .WithName("NotificationsServiceHealthCheck")
            .WithTags("Health");
    }
}
