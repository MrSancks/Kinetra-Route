using System.Collections.Generic;
using System.Linq;
using OrdersService.Application.Commands;
using OrdersService.Domain.Entities;
using OrdersService.Domain.ValueObjects;

namespace OrdersService.Api.Contracts;

public sealed record CreateOrderRequest(
    string RestaurantId,
    string CustomerId,
    string DeliveryAddress,
    decimal Total,
    GeoCoordinate DeliveryLocation,
    IReadOnlyList<OrderItemRequest> Items,
    bool AutoAssignRider = true)
{
    public CreateOrderCommand ToCommand() => new(
        RestaurantId,
        CustomerId,
        DeliveryAddress,
        Total,
        DeliveryLocation,
        Items.Select(i => new OrderItem(i.Sku, i.Name, i.Quantity, i.UnitPrice)).ToArray(),
        AutoAssignRider);
}

public sealed record OrderItemRequest(string Sku, string Name, int Quantity, decimal UnitPrice);

public sealed record UpdateOrderStatusRequest(OrderStatus Status)
{
    public UpdateOrderStatusCommand ToCommand(Guid orderId) => new(orderId, Status);
}

public sealed record OrderResponse(
    Guid Id,
    string RestaurantId,
    string CustomerId,
    string DeliveryAddress,
    decimal Total,
    OrderStatus Status,
    string? AssignedRiderId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    GeoCoordinate DeliveryLocation,
    IReadOnlyList<OrderItemResponse> Items)
{
    public static OrderResponse FromDomain(Order order) => new(
        order.Id,
        order.RestaurantId,
        order.CustomerId,
        order.DeliveryAddress,
        order.Total,
        order.Status,
        order.AssignedRiderId,
        order.CreatedAt,
        order.UpdatedAt,
        order.DeliveryLocation,
        order.Items.Select(i => new OrderItemResponse(i.Sku, i.Name, i.Quantity, i.UnitPrice)).ToArray());
}

public sealed record OrderItemResponse(string Sku, string Name, int Quantity, decimal UnitPrice);
