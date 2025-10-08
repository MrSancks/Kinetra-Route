using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RoutesService.Application.Contracts;
using RoutesService.Application.Services;

namespace RoutesService.Presentation;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/tracking").WithTags("Tracking");

        group.MapGet("/{riderId}", async (
            string riderId,
            IRiderTrackingService service,
            CancellationToken cancellationToken) =>
        {
            var location = await service.GetAsync(riderId, cancellationToken);
            return location is null ? Results.NotFound() : Results.Ok(location);
        })
        .WithName("GetRiderLocation")
        .WithSummary("Obtiene la ubicación actual del repartidor")
        .Produces<RiderLocationResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{riderId}", async (
            string riderId,
            TrackingUpdateRequest request,
            IRiderTrackingService service,
            CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.BadRequest();
            }

            var response = await service.UpdateAsync(riderId, request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("UpdateRiderLocation")
        .WithSummary("Actualiza la ubicación reportada por la app móvil del repartidor")
        .Produces<RiderLocationResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return builder;
    }
}
