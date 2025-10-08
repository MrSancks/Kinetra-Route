using System;
using System.Collections.Generic;

namespace RoutesService.Application.Contracts;

public sealed record GeoPointDto(double Latitude, double Longitude);

public sealed record RouteStopRequestDto(string Id, string Name, GeoPointDto Location);

public sealed record RouteOptimizationRequest(
    string? DeliveryId,
    GeoPointDto Origin,
    GeoPointDto Destination,
    IReadOnlyList<RouteStopRequestDto>? Waypoints,
    DateTimeOffset? DepartureTime,
    string? VehicleType);
