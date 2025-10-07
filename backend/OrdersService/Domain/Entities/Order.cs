using System.Collections.Generic;
using OrdersService.Domain.ValueObjects;

namespace OrdersService.Domain.Entities;

public record Order(
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
    IReadOnlyList<OrderItem> Items);
