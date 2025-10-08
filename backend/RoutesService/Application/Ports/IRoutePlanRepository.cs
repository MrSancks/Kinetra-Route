using System.Threading;
using System.Threading.Tasks;
using RoutesService.Domain.Entities;

namespace RoutesService.Application.Ports;

public interface IRoutePlanRepository
{
    Task SaveAsync(RoutePlan plan, CancellationToken cancellationToken);

    Task<RoutePlan?> GetByDeliveryIdAsync(string deliveryId, CancellationToken cancellationToken);
}
