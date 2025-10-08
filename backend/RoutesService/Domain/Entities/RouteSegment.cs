using System;

namespace RoutesService.Domain.Entities;

public sealed class RouteSegment
{
    public RouteSegment(string fromStopId, string toStopId, double distanceKm, TimeSpan duration, TimeSpan trafficDelay)
    {
        FromStopId = fromStopId;
        ToStopId = toStopId;
        DistanceKm = distanceKm;
        Duration = duration;
        TrafficDelay = trafficDelay;
    }

    public string FromStopId { get; }

    public string ToStopId { get; }

    public double DistanceKm { get; }

    public TimeSpan Duration { get; }

    public TimeSpan TrafficDelay { get; }
}
