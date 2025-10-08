using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;
using RoutesService.Domain.Entities;

namespace RoutesService.Application.Ports;

public interface IRouteOptimizationProvider
{
    Task<RoutePlan> OptimizeAsync(RouteOptimizationRequest request, CancellationToken cancellationToken);
}
