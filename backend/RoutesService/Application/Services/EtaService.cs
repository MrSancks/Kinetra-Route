using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;
using RoutesService.Domain.ValueObjects;

namespace RoutesService.Application.Services;

public sealed class EtaService : IEtaService
{
    private readonly IRoutePlanRepository _routePlanRepository;
    private readonly IRiderLocationRepository _riderLocationRepository;

    public EtaService(IRoutePlanRepository routePlanRepository, IRiderLocationRepository riderLocationRepository)
    {
        _routePlanRepository = routePlanRepository;
        _riderLocationRepository = riderLocationRepository;
    }

    public async Task<EtaResponse?> GetEtaAsync(string deliveryId, string riderId, CancellationToken cancellationToken)
    {
        var plan = await _routePlanRepository.GetByDeliveryIdAsync(deliveryId, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        var location = await _riderLocationRepository.GetAsync(riderId, cancellationToken);
        var progress = CalculateProgress(plan, location?.Coordinate);
        var totalDuration = plan.BaseDuration + plan.TrafficDelay;
        var remainingDuration = TimeSpan.FromTicks((long)(totalDuration.Ticks * (1 - progress)));
        if (remainingDuration < TimeSpan.Zero)
        {
            remainingDuration = TimeSpan.Zero;
        }

        var eta = DateTimeOffset.UtcNow + remainingDuration;
        return new EtaResponse(deliveryId, riderId, eta, remainingDuration, Math.Round(progress * 100, 2));
    }

    private static double CalculateProgress(RoutePlan plan, GeoCoordinate? coordinate)
    {
        if (coordinate is null)
        {
            return 0d;
        }

        var orderedStops = plan.Stops.OrderBy(stop => stop.Sequence).ToArray();
        if (orderedStops.Length < 2)
        {
            return 0d;
        }

        var segments = plan.Segments.ToArray();
        var totalDistance = segments.Sum(segment => segment.DistanceKm);
        if (totalDistance <= 0)
        {
            totalDistance = Math.Max(plan.TotalDistanceKm, 0d);
        }

        if (totalDistance <= 0)
        {
            return 0d;
        }

        var nearestStop = orderedStops
            .OrderBy(stop => stop.Location.DistanceToKm(coordinate.Value))
            .First();

        if (nearestStop.Type == RouteStopType.Destination)
        {
            return 1d;
        }

        if (nearestStop.Type == RouteStopType.Origin)
        {
            return 0d;
        }

        var stopById = orderedStops.ToDictionary(stop => stop.Id);
        var completedDistance = 0d;

        foreach (var segment in segments)
        {
            if (!stopById.TryGetValue(segment.ToStopId, out var toStop))
            {
                continue;
            }

            if (toStop.Sequence < nearestStop.Sequence)
            {
                completedDistance += Math.Max(segment.DistanceKm, 0);
            }
        }

        var previousStop = orderedStops.LastOrDefault(stop => stop.Sequence < nearestStop.Sequence);
        if (previousStop is not null)
        {
            var currentSegment = segments.FirstOrDefault(segment =>
                segment.FromStopId == previousStop.Id && segment.ToStopId == nearestStop.Id);

            if (currentSegment is not null && currentSegment.DistanceKm > 0)
            {
                var distanceToNearestStop = coordinate.Value.DistanceToKm(nearestStop.Location);
                var partial = 1 - Math.Clamp(distanceToNearestStop / currentSegment.DistanceKm, 0, 1);
                completedDistance += currentSegment.DistanceKm * partial;
            }
        }

        var progress = completedDistance / totalDistance;
        return Math.Clamp(progress, 0d, 1d);
    }
}
