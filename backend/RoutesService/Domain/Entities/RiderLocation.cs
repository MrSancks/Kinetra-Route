using System;
using RoutesService.Domain.ValueObjects;

namespace RoutesService.Domain.Entities;

public sealed class RiderLocation
{
    public RiderLocation(string riderId, GeoCoordinate coordinate, DateTimeOffset recordedAt)
    {
        RiderId = riderId;
        Coordinate = coordinate;
        RecordedAt = recordedAt;
    }

    public string RiderId { get; }

    public GeoCoordinate Coordinate { get; }

    public DateTimeOffset RecordedAt { get; }
}
