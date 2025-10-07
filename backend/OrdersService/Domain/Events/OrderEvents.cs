using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Events;

public record OrderCreatedEvent(Guid OrderId, string RestaurantId, string CustomerId, decimal Total, DateTime OccurredAt)
    : IntegrationEvent(OccurredAt);

public record OrderAssignedEvent(Guid OrderId, string RiderId, DateTime OccurredAt) : IntegrationEvent(OccurredAt);

public record OrderStatusChangedEvent(Guid OrderId, OrderStatus Status, DateTime OccurredAt) : IntegrationEvent(OccurredAt);
