using System.Collections.Generic;
using System.Linq;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Events;

namespace OrdersService.Application.Commands;

public sealed class OrderCommandService : IOrderCommandService
{
    private static readonly IReadOnlyDictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new Dictionary<OrderStatus, OrderStatus[]>
    {
        [OrderStatus.Created] = new[] { OrderStatus.Assigned, OrderStatus.Cancelled },
        [OrderStatus.Assigned] = new[] { OrderStatus.EnRoute, OrderStatus.Cancelled },
        [OrderStatus.EnRoute] = new[] { OrderStatus.Delivered, OrderStatus.Cancelled },
        [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
        [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
    };

    private readonly IOrderRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IRestaurantIntegration _restaurantIntegration;
    private readonly IRiderAssignmentService _riderAssignmentService;
    private readonly IClock _clock;

    public OrderCommandService(
        IOrderRepository repository,
        IEventPublisher eventPublisher,
        IRestaurantIntegration restaurantIntegration,
        IRiderAssignmentService riderAssignmentService,
        IClock clock)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _restaurantIntegration = restaurantIntegration;
        _riderAssignmentService = riderAssignmentService;
        _clock = clock;
    }

    public async Task<Order> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        if (command.Items.Count == 0)
        {
            throw new ArgumentException("An order must contain at least one item", nameof(command));
        }

        var restaurantAccepted = await _restaurantIntegration.ValidateOrderAsync(command.RestaurantId, command.Items, cancellationToken);
        if (!restaurantAccepted)
        {
            throw new InvalidOperationException("The restaurant rejected the order");
        }

        var now = _clock.UtcNow;
        var order = new Order(
            Guid.NewGuid(),
            command.RestaurantId,
            command.CustomerId,
            command.DeliveryAddress,
            command.Total,
            OrderStatus.Created,
            null,
            now,
            null,
            command.DeliveryLocation,
            command.Items);

        await _repository.AddAsync(order, cancellationToken);
        await _eventPublisher.PublishAsync(new OrderCreatedEvent(order.Id, order.RestaurantId, order.CustomerId, order.Total, now), cancellationToken);

        if (!command.AutoAssignRider)
        {
            return order;
        }

        return await AssignRiderInternalAsync(order.Id, order, cancellationToken) ?? order;
    }

    public async Task<Order?> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var updated = await _repository.UpdateAsync(command.OrderId, current =>
        {
            if (current.Status == command.Status)
            {
                return current with { UpdatedAt = _clock.UtcNow };
            }

            if (!AllowedTransitions.TryGetValue(current.Status, out var allowed) || !allowed.Contains(command.Status))
            {
                throw new InvalidOperationException($"Transition from {current.Status} to {command.Status} is not allowed");
            }

            return current with { Status = command.Status, UpdatedAt = _clock.UtcNow };
        }, cancellationToken);

        if (updated is null)
        {
            return null;
        }

        await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(updated.Id, updated.Status, _clock.UtcNow), cancellationToken);

        if (updated.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            await _riderAssignmentService.ReleaseRiderAsync(updated.Id, updated.AssignedRiderId, cancellationToken);
        }

        return updated;
    }

    public async Task<Order?> ReassignAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _repository.GetAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        await _riderAssignmentService.ReleaseRiderAsync(order.Id, order.AssignedRiderId, cancellationToken);

        var reset = await _repository.UpdateAsync(orderId, _ => order with
        {
            AssignedRiderId = null,
            Status = OrderStatus.Created,
            UpdatedAt = _clock.UtcNow
        }, cancellationToken);

        if (reset is not null)
        {
            await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(reset.Id, reset.Status, _clock.UtcNow), cancellationToken);
        }

        return await AssignRiderInternalAsync(orderId, reset ?? order, cancellationToken);
    }

    public async Task<bool> CancelAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _repository.GetAsync(orderId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            return true;
        }

        var updated = await HandleAsync(new UpdateOrderStatusCommand(orderId, OrderStatus.Cancelled), cancellationToken);
        return updated is not null;
    }

    private async Task<Order?> AssignRiderInternalAsync(Guid orderId, Order existingOrder, CancellationToken cancellationToken)
    {
        var assignedRider = await _riderAssignmentService.AssignNearestRiderAsync(existingOrder, cancellationToken);
        if (assignedRider is null)
        {
            return existingOrder;
        }

        var updated = await _repository.UpdateAsync(orderId, current => current with
        {
            Status = OrderStatus.Assigned,
            AssignedRiderId = assignedRider,
            UpdatedAt = _clock.UtcNow
        }, cancellationToken);

        if (updated is not null)
        {
            await _eventPublisher.PublishAsync(new OrderAssignedEvent(updated.Id, assignedRider, _clock.UtcNow), cancellationToken);
        }

        return updated;
    }
}
