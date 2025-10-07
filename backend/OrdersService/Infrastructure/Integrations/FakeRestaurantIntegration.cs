using System.Linq;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;

namespace OrdersService.Infrastructure.Integrations;

public sealed class FakeRestaurantIntegration : IRestaurantIntegration
{
    public Task<bool> ValidateOrderAsync(string restaurantId, IReadOnlyList<OrderItem> items, CancellationToken cancellationToken)
    {
        var totalItems = items.Sum(i => i.Quantity);
        return Task.FromResult(totalItems <= 50);
    }
}
