using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;

namespace RoutesService.Application.Services;

public interface IRiderTrackingService
{
    Task<RiderLocationResponse?> GetAsync(string riderId, CancellationToken cancellationToken);

    Task<RiderLocationResponse> UpdateAsync(string riderId, TrackingUpdateRequest request, CancellationToken cancellationToken);
}
