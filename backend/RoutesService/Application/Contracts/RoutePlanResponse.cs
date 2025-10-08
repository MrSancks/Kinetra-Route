using System;
using System.Collections.Generic;

namespace RoutesService.Application.Contracts;

public sealed record RoutePlanResponse(
    string? DeliveryId,
    double TotalDistanceKm,
    TimeSpan TravelDuration,
    TimeSpan TrafficDelay,
    DateTimeOffset DepartureTime,
    DateTimeOffset EstimatedArrival,
    IReadOnlyList<RouteStopResponse> Stops,
    IReadOnlyList<RouteSegmentResponse> Segments);

public sealed record RouteStopResponse(
    string Id,
    string Name,
    string Type,
    GeoPointDto Location,
    int Sequence);

public sealed record RouteSegmentResponse(
    string FromStopId,
    string ToStopId,
    double DistanceKm,
    TimeSpan Duration,
    TimeSpan TrafficDelay);
