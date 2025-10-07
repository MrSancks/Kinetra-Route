using OrdersService.Domain.Entities;

namespace OrdersService.Application.Commands;

public interface IOrderCommandService
{
    Task<Order> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken);
    Task<Order?> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken);
    Task<Order?> ReassignAsync(Guid orderId, CancellationToken cancellationToken);
    Task<bool> CancelAsync(Guid orderId, CancellationToken cancellationToken);
}
