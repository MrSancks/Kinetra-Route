using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;

namespace RoutesService.Infrastructure.Repositories;

public sealed class InMemoryRoutePlanRepository : IRoutePlanRepository
{
    private readonly ConcurrentDictionary<string, RoutePlan> _plans = new();

    public Task SaveAsync(RoutePlan plan, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(plan.DeliveryId))
        {
            _plans[plan.DeliveryId] = plan;
        }

        return Task.CompletedTask;
    }

    public Task<RoutePlan?> GetByDeliveryIdAsync(string deliveryId, CancellationToken cancellationToken)
    {
        _plans.TryGetValue(deliveryId, out var plan);
        return Task.FromResult(plan);
    }
}
