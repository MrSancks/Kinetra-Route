using System.Collections.Concurrent;
using System.Linq;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;

namespace OrdersService.Infrastructure.Persistence;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"The order {order.Id} already exists");
        }

        return Task.CompletedTask;
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_orders.TryGetValue(id, out var order) ? order : null);

    public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult((IReadOnlyCollection<Order>)_orders.Values.OrderByDescending(o => o.CreatedAt).ToArray());

    public Task<Order?> UpdateAsync(Guid id, Func<Order, Order> update, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (!_orders.TryGetValue(id, out var current))
            {
                return Task.FromResult<Order?>(null);
            }

            var updated = update(current);
            if (_orders.TryUpdate(id, updated, current))
            {
                return Task.FromResult<Order?>(updated);
            }
        }
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_orders.TryRemove(id, out _));
}
