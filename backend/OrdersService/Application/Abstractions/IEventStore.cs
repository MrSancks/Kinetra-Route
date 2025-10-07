using OrdersService.Domain.Events;

namespace OrdersService.Application.Abstractions;

public interface IEventStore
{
    IReadOnlyCollection<IIntegrationEvent> GetEvents();
}
