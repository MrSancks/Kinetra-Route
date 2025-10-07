using OrdersService.Domain.Events;

namespace OrdersService.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
