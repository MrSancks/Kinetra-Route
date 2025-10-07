using OrdersService.Domain.Entities;

namespace OrdersService.Application.Commands;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status);
