using System.Collections.Generic;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken);
    Task<Order?> UpdateAsync(Guid id, Func<Order, Order> update, CancellationToken cancellationToken);
    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken);
}
