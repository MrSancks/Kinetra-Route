namespace OrdersService.Domain.Events;

public interface IIntegrationEvent
{
    DateTime OccurredAt { get; }
    string EventName { get; }
}
