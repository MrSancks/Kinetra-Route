using System.Collections.Concurrent;
using System.Linq;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;
using OrdersService.Domain.ValueObjects;

namespace OrdersService.Infrastructure.Routing;

public sealed class InMemoryRiderAssignmentService : IRiderAssignmentService
{
    private readonly ConcurrentDictionary<string, RiderState> _riders = new();

    public InMemoryRiderAssignmentService()
    {
        var seed = new[]
        {
            new RiderState("rider-1", new GeoCoordinate(40.4168, -3.7038)),
            new RiderState("rider-2", new GeoCoordinate(41.3874, 2.1686)),
            new RiderState("rider-3", new GeoCoordinate(37.3891, -5.9845))
        };

        foreach (var rider in seed)
        {
            _riders.TryAdd(rider.Id, rider);
        }
    }

    public Task<string?> AssignNearestRiderAsync(Order order, CancellationToken cancellationToken)
    {
        var available = _riders.Values.Where(r => r.IsAvailable).ToArray();
        if (available.Length == 0)
        {
            return Task.FromResult<string?>(null);
        }

        var nearest = available.OrderBy(r => r.Location.DistanceTo(order.DeliveryLocation)).First();
        _riders.AddOrUpdate(nearest.Id, nearest with { IsAvailable = false, CurrentOrderId = order.Id }, (_, _) => nearest with
        {
            IsAvailable = false,
            CurrentOrderId = order.Id
        });

        return Task.FromResult<string?>(nearest.Id);
    }

    public Task ReleaseRiderAsync(Guid orderId, string? riderId, CancellationToken cancellationToken)
    {
        if (riderId is null)
        {
            return Task.CompletedTask;
        }

        _riders.AddOrUpdate(
            riderId,
            id => new RiderState(id, new GeoCoordinate(0, 0)),
            (_, existing) => existing.CurrentOrderId == orderId
                ? existing with { IsAvailable = true, CurrentOrderId = null }
                : existing);

        return Task.CompletedTask;
    }

    private sealed record RiderState(string Id, GeoCoordinate Location)
    {
        public bool IsAvailable { get; init; } = true;
        public Guid? CurrentOrderId { get; init; }
            = null;
    }
}
