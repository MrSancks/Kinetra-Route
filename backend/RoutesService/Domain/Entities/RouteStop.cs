using RoutesService.Domain.ValueObjects;

namespace RoutesService.Domain.Entities;

public enum RouteStopType
{
    Origin,
    Waypoint,
    Destination
}

public sealed class RouteStop
{
    public RouteStop(string id, string name, GeoCoordinate location, RouteStopType type, int sequence)
    {
        Id = id;
        Name = name;
        Location = location;
        Type = type;
        Sequence = sequence;
    }

    public string Id { get; }

    public string Name { get; }

    public GeoCoordinate Location { get; }

    public RouteStopType Type { get; }

    public int Sequence { get; }
}
