using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;
using RoutesService.Domain.ValueObjects;
using RoutesService.Infrastructure.Data;
using RoutesService.Infrastructure.Data.Models;

namespace RoutesService.Infrastructure.Repositories;

public sealed class RiderLocationRepository : IRiderLocationRepository
{
    private readonly RoutesDbContext _context;

    public RiderLocationRepository(RoutesDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(RiderLocation location, CancellationToken cancellationToken)
    {
        var record = await _context.RiderLocations
            .FirstOrDefaultAsync(r => r.RiderId == location.RiderId, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            record = new RiderLocationRecord
            {
                RiderId = location.RiderId,
                Latitude = location.Coordinate.Latitude,
                Longitude = location.Coordinate.Longitude,
                RecordedAt = location.RecordedAt
            };

            await _context.RiderLocations.AddAsync(record, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            record.Latitude = location.Coordinate.Latitude;
            record.Longitude = location.Coordinate.Longitude;
            record.RecordedAt = location.RecordedAt;
            _context.RiderLocations.Update(record);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<RiderLocation?> GetAsync(string riderId, CancellationToken cancellationToken)
    {
        var record = await _context.RiderLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RiderId == riderId, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            return null;
        }

        var coordinate = new GeoCoordinate(record.Latitude, record.Longitude);
        return new RiderLocation(record.RiderId, coordinate, record.RecordedAt);
    }
}
