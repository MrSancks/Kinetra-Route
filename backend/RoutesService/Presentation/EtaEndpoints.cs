using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RoutesService.Application.Contracts;
using RoutesService.Application.Services;

namespace RoutesService.Presentation;

public static class EtaEndpoints
{
    public static IEndpointRouteBuilder MapEtaEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/deliveries").WithTags("Deliveries");

        group.MapGet("/{deliveryId}/eta", async (
            string deliveryId,
            string riderId,
            IEtaService etaService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(riderId))
            {
                return Results.BadRequest("Se requiere el identificador del repartidor.");
            }

            var eta = await etaService.GetEtaAsync(deliveryId, riderId, cancellationToken);
            return eta is null ? Results.NotFound() : Results.Ok(eta);
        })
        .WithName("GetDeliveryEta")
        .WithSummary("Calcula el tiempo estimado de llegada (ETA) de una entrega")
        .Produces<EtaResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return builder;
    }
}
