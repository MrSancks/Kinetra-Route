using System;

namespace RoutesService.Domain.ValueObjects;

public readonly record struct GeoCoordinate(double Latitude, double Longitude)
{
    public double Latitude { get; } = Latitude;
    public double Longitude { get; } = Longitude;

    public double DistanceToKm(GeoCoordinate other)
    {
        const double EarthRadiusKm = 6371.0;
        var lat1Rad = DegreesToRadians(Latitude);
        var lat2Rad = DegreesToRadians(other.Latitude);
        var deltaLat = DegreesToRadians(other.Latitude - Latitude);
        var deltaLon = DegreesToRadians(other.Longitude - Longitude);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
