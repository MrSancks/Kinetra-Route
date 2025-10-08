using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RoutesService.Application.Contracts;
using RoutesService.Application.Services;

namespace RoutesService.Presentation;

public static class RouteEndpoints
{
    public static IEndpointRouteBuilder MapRouteEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/routes").WithTags("Routes");

        group.MapPost("/optimize", async (
            RouteOptimizationRequest request,
            IRoutePlanningService service,
            CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.BadRequest();
            }

            var response = await service.OptimizeAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("OptimizeRoute")
        .WithSummary("Calcula la ruta óptima para una entrega")
        .Produces<RoutePlanResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return builder;
    }
}
