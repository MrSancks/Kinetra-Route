using System.Collections.Generic;
using System.Threading;
using ReportsService.Domain.Events;

namespace ReportsService.Application.Abstractions;

public interface IReportEventRepository
{
    Task SaveAsync(OrderCompletedEvent @event, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderCompletedEvent>> LoadAllAsync(CancellationToken cancellationToken);
}
