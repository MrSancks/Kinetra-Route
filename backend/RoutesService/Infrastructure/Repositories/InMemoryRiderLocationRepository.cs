using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;

namespace RoutesService.Infrastructure.Repositories;

public sealed class InMemoryRiderLocationRepository : IRiderLocationRepository
{
    private readonly ConcurrentDictionary<string, RiderLocation> _locations = new();

    public Task SaveAsync(RiderLocation location, CancellationToken cancellationToken)
    {
        _locations[location.RiderId] = location;
        return Task.CompletedTask;
    }

    public Task<RiderLocation?> GetAsync(string riderId, CancellationToken cancellationToken)
    {
        _locations.TryGetValue(riderId, out var location);
        return Task.FromResult(location);
    }
}
