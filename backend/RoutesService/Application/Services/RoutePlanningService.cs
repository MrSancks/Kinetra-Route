using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;

namespace RoutesService.Application.Services;

public sealed class RoutePlanningService : IRoutePlanningService
{
    private readonly IRouteOptimizationProvider _routeOptimizationProvider;
    private readonly IRoutePlanRepository _routePlanRepository;

    public RoutePlanningService(IRouteOptimizationProvider routeOptimizationProvider, IRoutePlanRepository routePlanRepository)
    {
        _routeOptimizationProvider = routeOptimizationProvider;
        _routePlanRepository = routePlanRepository;
    }

    public async Task<RoutePlanResponse> OptimizeAsync(RouteOptimizationRequest request, CancellationToken cancellationToken)
    {
        var plan = await _routeOptimizationProvider.OptimizeAsync(request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(plan.DeliveryId))
        {
            await _routePlanRepository.SaveAsync(plan, cancellationToken);
        }

        return MapToResponse(plan);
    }

    private static RoutePlanResponse MapToResponse(RoutePlan plan)
    {
        return new RoutePlanResponse(
            plan.DeliveryId,
            plan.TotalDistanceKm,
            plan.BaseDuration,
            plan.TrafficDelay,
            plan.DepartureTime,
            plan.EstimatedArrival,
            plan.Stops.Select(stop => new RouteStopResponse(
                stop.Id,
                stop.Name,
                stop.Type.ToString(),
                new GeoPointDto(stop.Location.Latitude, stop.Location.Longitude),
                stop.Sequence)).ToArray(),
            plan.Segments.Select(segment => new RouteSegmentResponse(
                segment.FromStopId,
                segment.ToStopId,
                segment.DistanceKm,
                segment.Duration,
                segment.TrafficDelay)).ToArray());
    }
}
