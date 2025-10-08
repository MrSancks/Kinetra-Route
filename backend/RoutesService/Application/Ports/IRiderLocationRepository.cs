using System.Threading;
using System.Threading.Tasks;
using RoutesService.Domain.Entities;

namespace RoutesService.Application.Ports;

public interface IRiderLocationRepository
{
    Task SaveAsync(RiderLocation location, CancellationToken cancellationToken);

    Task<RiderLocation?> GetAsync(string riderId, CancellationToken cancellationToken);
}
