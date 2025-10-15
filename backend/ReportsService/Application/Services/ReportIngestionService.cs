using ReportsService.Application.Abstractions;
using ReportsService.Application.MaterializedViews;
using ReportsService.Contracts.Requests;
using ReportsService.Domain.Events;

namespace ReportsService.Application.Services;

public sealed class ReportIngestionService
{
    private readonly ReportMaterializedViewStore _viewStore;
    private readonly IReportEventRepository _repository;

    public ReportIngestionService(ReportMaterializedViewStore viewStore, IReportEventRepository repository)
    {
        _viewStore = viewStore;
        _repository = repository;
    }

    public async ValueTask RecordOrderCompletionAsync(RecordOrderCompletionRequest request, CancellationToken cancellationToken = default)
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

        await _repository.SaveAsync(@event, cancellationToken).ConfigureAwait(false);
        _viewStore.Apply(@event);
    }
}
