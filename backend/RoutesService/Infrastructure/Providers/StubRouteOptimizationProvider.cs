using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;
using RoutesService.Domain.ValueObjects;

namespace RoutesService.Infrastructure.Providers;

public sealed class StubRouteOptimizationProvider : IRouteOptimizationProvider
{
    private const double DefaultAverageSpeedKmh = 38d;

    public Task<RoutePlan> OptimizeAsync(RouteOptimizationRequest request, CancellationToken cancellationToken)
    {
        var departureTime = request.DepartureTime ?? DateTimeOffset.UtcNow;

        var waypoints = request.Waypoints?.ToList() ?? new List<RouteStopRequestDto>();
        var stops = BuildStops(request, waypoints);
        var segments = BuildSegments(stops);

        var totalDistance = segments.Sum(segment => segment.DistanceKm);
        var baseDuration = CalculateDuration(totalDistance);
        var trafficDelay = CalculateTrafficDelay(departureTime, totalDistance, request.VehicleType);

        var plan = new RoutePlan(
            request.DeliveryId,
            stops,
            segments,
            totalDistance,
            baseDuration,
            trafficDelay,
            departureTime);

        return Task.FromResult(plan);
    }

    private static IReadOnlyList<RouteStop> BuildStops(RouteOptimizationRequest request, IReadOnlyList<RouteStopRequestDto> waypoints)
    {
        var stops = new List<RouteStop>();
        var originCoordinate = new GeoCoordinate(request.Origin.Latitude, request.Origin.Longitude);
        stops.Add(new RouteStop(
            request.DeliveryId is null ? "origin" : $"{request.DeliveryId}-origin",
            "Punto de partida",
            originCoordinate,
            RouteStopType.Origin,
            0));

        for (var index = 0; index < waypoints.Count; index++)
        {
            var waypoint = waypoints[index];
            var coordinate = new GeoCoordinate(waypoint.Location.Latitude, waypoint.Location.Longitude);
            stops.Add(new RouteStop(
                waypoint.Id,
                string.IsNullOrWhiteSpace(waypoint.Name) ? $"Parada {index + 1}" : waypoint.Name,
                coordinate,
                RouteStopType.Waypoint,
                index + 1));
        }

        var destinationCoordinate = new GeoCoordinate(request.Destination.Latitude, request.Destination.Longitude);
        stops.Add(new RouteStop(
            request.DeliveryId is null ? "destination" : $"{request.DeliveryId}-destination",
            "Destino",
            destinationCoordinate,
            RouteStopType.Destination,
            waypoints.Count + 1));

        return stops;
    }

    private static IReadOnlyList<RouteSegment> BuildSegments(IReadOnlyList<RouteStop> stops)
    {
        var segments = new List<RouteSegment>();
        for (var index = 0; index < stops.Count - 1; index++)
        {
            var from = stops[index];
            var to = stops[index + 1];
            var distance = from.Location.DistanceToKm(to.Location);
            var duration = CalculateDuration(distance);
            var trafficDelay = TimeSpan.Zero;
            segments.Add(new RouteSegment(from.Id, to.Id, Math.Round(distance, 2), duration, trafficDelay));
        }

        return segments;
    }

    private static TimeSpan CalculateDuration(double distanceKm)
    {
        if (distanceKm <= 0)
        {
            return TimeSpan.Zero;
        }

        var hours = distanceKm / DefaultAverageSpeedKmh;
        return TimeSpan.FromHours(hours);
    }

    private static TimeSpan CalculateTrafficDelay(DateTimeOffset departureTime, double totalDistanceKm, string? vehicleType)
    {
        var rushHours = departureTime.Hour is >= 7 and < 10 or >= 17 and < 20;
        var delayFactor = rushHours ? 0.25 : 0.1;
        if (string.Equals(vehicleType, "bike", StringComparison.OrdinalIgnoreCase))
        {
            delayFactor *= 0.6;
        }

        var delayHours = (totalDistanceKm / DefaultAverageSpeedKmh) * delayFactor;
        return TimeSpan.FromHours(delayHours);
    }
}
