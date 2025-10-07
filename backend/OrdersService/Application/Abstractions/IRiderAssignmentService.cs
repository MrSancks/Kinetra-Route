using OrdersService.Domain.Entities;

namespace OrdersService.Application.Abstractions;

public interface IRiderAssignmentService
{
    Task<string?> AssignNearestRiderAsync(Order order, CancellationToken cancellationToken);
    Task ReleaseRiderAsync(Guid orderId, string? riderId, CancellationToken cancellationToken);
}
