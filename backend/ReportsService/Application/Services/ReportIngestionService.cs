using ReportsService.Application.MaterializedViews;
using ReportsService.Contracts.Requests;
using ReportsService.Domain.Events;

namespace ReportsService.Application.Services;

public sealed class ReportIngestionService
{
    private readonly ReportMaterializedViewStore _viewStore;

    public ReportIngestionService(ReportMaterializedViewStore viewStore)
    {
        _viewStore = viewStore;
    }

    public ValueTask RecordOrderCompletionAsync(RecordOrderCompletionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var @event = new OrderCompletedEvent(
            request.OrderId,
            request.RiderId,
            request.CompletedAt,
            request.OrderTotal,
            request.PlatformFee,
            request.DeliveredOnTime,
            request.DeliveryDurationMinutes);

        _viewStore.Apply(@event);

        return ValueTask.CompletedTask;
    }
}
