using System;

namespace RoutesService.Application.Contracts;

public sealed record RiderLocationResponse(
    string RiderId,
    GeoPointDto Location,
    DateTimeOffset RecordedAt);

public sealed record TrackingUpdateRequest(
    double Latitude,
    double Longitude,
    DateTimeOffset? RecordedAt);
