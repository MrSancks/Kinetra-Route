using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using OrdersService.Api.Contracts;
using OrdersService.Application.Abstractions;
using OrdersService.Application.Commands;
using OrdersService.Application.Queries;

namespace OrdersService.Presentation;

public static class PresentationConfiguration
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IEndpointRouteBuilder MapPresentationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", () => Results.Ok(new { status = "OK", service = "OrdersService" }));

        endpoints.MapOrderEndpoints();
        endpoints.MapEventEndpoints();

        return endpoints;
    }

    private static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routes = endpoints.MapGroup("/orders");

        routes.MapGet(string.Empty, async (IOrderQueryService queryService, CancellationToken cancellationToken) =>
        {
            var orderCollection = await queryService.GetAllAsync(cancellationToken);
            return Results.Ok(orderCollection.Select(OrderResponse.FromDomain));
        });

        routes.MapGet("/{id:guid}", async (Guid id, IOrderQueryService queryService, CancellationToken cancellationToken) =>
        {
            var order = await queryService.GetByIdAsync(id, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(OrderResponse.FromDomain(order));
        });

        routes.MapPost(string.Empty, async (CreateOrderRequest request, IOrderCommandService commandService, CancellationToken cancellationToken) =>
        {
            var order = await commandService.HandleAsync(request.ToCommand(), cancellationToken);
            return Results.Created($"/orders/{order.Id}", OrderResponse.FromDomain(order));
        });

        routes.MapPatch("/{id:guid}/status", async (Guid id, UpdateOrderStatusRequest request, IOrderCommandService commandService, CancellationToken cancellationToken) =>
        {
            var order = await commandService.HandleAsync(request.ToCommand(id), cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(OrderResponse.FromDomain(order));
        });

        routes.MapPost("/{id:guid}/reassign", async (Guid id, IOrderCommandService commandService, CancellationToken cancellationToken) =>
        {
            var order = await commandService.ReassignAsync(id, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(OrderResponse.FromDomain(order));
        });

        routes.MapDelete("/{id:guid}", async (Guid id, IOrderCommandService commandService, CancellationToken cancellationToken) =>
        {
            var cancelled = await commandService.CancelAsync(id, cancellationToken);
            return cancelled ? Results.NoContent() : Results.NotFound();
        });

        return endpoints;
    }

    private static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/events", (IEventStore eventStore) => Results.Ok(eventStore.GetEvents()));
        return endpoints;
    }
}
