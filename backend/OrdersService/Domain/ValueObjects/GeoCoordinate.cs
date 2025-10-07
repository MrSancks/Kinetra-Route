namespace OrdersService.Domain.ValueObjects;

public readonly record struct GeoCoordinate(double Latitude, double Longitude)
{
    public double DistanceTo(GeoCoordinate other)
    {
        var dLat = Latitude - other.Latitude;
        var dLon = Longitude - other.Longitude;
        return Math.Sqrt((dLat * dLat) + (dLon * dLon));
    }
}
