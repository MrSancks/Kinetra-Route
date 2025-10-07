using System.Collections.Concurrent;
using System.Collections.Generic;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Events;

namespace OrdersService.Infrastructure.Messaging;

public sealed class InMemoryEventPublisher : IEventPublisher, IEventStore
{
    private readonly ConcurrentQueue<IIntegrationEvent> _events = new();

    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        _events.Enqueue(integrationEvent);
        return Task.CompletedTask;
    }

    public IReadOnlyCollection<IIntegrationEvent> GetEvents() => _events.ToArray();
}
