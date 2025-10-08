using System;
using System.Collections.Generic;
using System.Linq;

namespace RoutesService.Domain.Entities;

public sealed class RoutePlan
{
    public RoutePlan(
        string? deliveryId,
        IReadOnlyList<RouteStop> stops,
        IReadOnlyList<RouteSegment> segments,
        double totalDistanceKm,
        TimeSpan baseDuration,
        TimeSpan trafficDelay,
        DateTimeOffset departureTime)
    {
        DeliveryId = deliveryId;
        Stops = stops;
        Segments = segments;
        TotalDistanceKm = totalDistanceKm;
        BaseDuration = baseDuration;
        TrafficDelay = trafficDelay;
        DepartureTime = departureTime;
        EstimatedArrival = departureTime + baseDuration + trafficDelay;
    }

    public string? DeliveryId { get; }

    public IReadOnlyList<RouteStop> Stops { get; }

    public IReadOnlyList<RouteSegment> Segments { get; }

    public double TotalDistanceKm { get; }

    public TimeSpan BaseDuration { get; }

    public TimeSpan TrafficDelay { get; }

    public DateTimeOffset DepartureTime { get; }

    public DateTimeOffset EstimatedArrival { get; }

    public RouteStop Origin => Stops.First(s => s.Type == RouteStopType.Origin);

    public RouteStop Destination => Stops.First(s => s.Type == RouteStopType.Destination);
}
