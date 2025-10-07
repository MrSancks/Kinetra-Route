using System.Collections.Generic;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Queries;

public interface IOrderQueryService
{
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
}
