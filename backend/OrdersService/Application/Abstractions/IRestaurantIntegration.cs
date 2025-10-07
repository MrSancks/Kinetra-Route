using System.Collections.Generic;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Abstractions;

public interface IRestaurantIntegration
{
    Task<bool> ValidateOrderAsync(string restaurantId, IReadOnlyList<OrderItem> items, CancellationToken cancellationToken);
}
