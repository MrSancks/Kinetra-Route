using System.Linq;
using Microsoft.EntityFrameworkCore;
using RidersService.Domain.Entities;
using RidersService.Domain.Enums;
using RidersService.Infrastructure.Data;

namespace RidersService.Infrastructure.Repositories;

public class RiderRepository : GenericRepository<Rider>, IRiderRepository
{
    public RiderRepository(RidersDbContext context) : base(context)
    {
    }

    public async Task<Rider?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Riders
            .Include(r => r.Vehicle)
            .Include(r => r.Documents)
            .Include(r => r.Deliveries)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rider>> GetAvailableRidersAsync(CancellationToken cancellationToken = default)
    {
        var riders = await Context.Riders
            .Where(r => r.Availability == AvailabilityStatus.Online)
            .Include(r => r.Vehicle)
            .Include(r => r.Documents)
            .Include(r => r.Deliveries)
            .ToListAsync(cancellationToken);

        return riders;
    }

    public async Task<IReadOnlyCollection<Rider>> GetAllDetailedAsync(CancellationToken cancellationToken = default)
    {
        var riders = await Context.Riders
            .Include(r => r.Vehicle)
            .Include(r => r.Documents)
            .Include(r => r.Deliveries)
            .ToListAsync(cancellationToken);

        return riders;
    }

    public override async Task<Rider?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Riders.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
