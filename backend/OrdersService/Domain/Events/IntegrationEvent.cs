namespace OrdersService.Domain.Events;

public abstract record IntegrationEvent(DateTime OccurredAt) : IIntegrationEvent
{
    public string EventName => GetType().Name;
}
