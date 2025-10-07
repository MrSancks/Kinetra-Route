using System.Collections.Generic;
using OrdersService.Domain.Entities;
using OrdersService.Domain.ValueObjects;

namespace OrdersService.Application.Commands;

public sealed record CreateOrderCommand(
    string RestaurantId,
    string CustomerId,
    string DeliveryAddress,
    decimal Total,
    GeoCoordinate DeliveryLocation,
    IReadOnlyList<OrderItem> Items,
    bool AutoAssignRider);
