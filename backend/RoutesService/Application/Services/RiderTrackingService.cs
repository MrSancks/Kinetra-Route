using System;
using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;
using RoutesService.Domain.ValueObjects;

namespace RoutesService.Application.Services;

public sealed class RiderTrackingService : IRiderTrackingService
{
    private readonly IRiderLocationRepository _repository;

    public RiderTrackingService(IRiderLocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<RiderLocationResponse?> GetAsync(string riderId, CancellationToken cancellationToken)
    {
        var location = await _repository.GetAsync(riderId, cancellationToken);
        return location is null ? null : Map(location);
    }

    public async Task<RiderLocationResponse> UpdateAsync(string riderId, TrackingUpdateRequest request, CancellationToken cancellationToken)
    {
        var coordinate = new GeoCoordinate(request.Latitude, request.Longitude);
        var recordedAt = request.RecordedAt ?? DateTimeOffset.UtcNow;
        var location = new RiderLocation(riderId, coordinate, recordedAt);
        await _repository.SaveAsync(location, cancellationToken);
        return Map(location);
    }

    private static RiderLocationResponse Map(RiderLocation location) =>
        new(location.RiderId, new GeoPointDto(location.Coordinate.Latitude, location.Coordinate.Longitude), location.RecordedAt);
}
