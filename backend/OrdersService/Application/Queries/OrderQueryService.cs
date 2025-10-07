using System.Collections.Generic;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Queries;

public sealed class OrderQueryService : IOrderQueryService
{
    private readonly IOrderRepository _repository;

    public OrderQueryService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken)
        => _repository.GetAllAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => _repository.GetAsync(orderId, cancellationToken);
}
