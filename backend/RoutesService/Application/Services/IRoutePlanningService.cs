using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;

namespace RoutesService.Application.Services;

public interface IRoutePlanningService
{
    Task<RoutePlanResponse> OptimizeAsync(RouteOptimizationRequest request, CancellationToken cancellationToken);
}
