using RidersService.Domain.Entities;

namespace RidersService.Infrastructure.Repositories;

public interface IRiderRepository : IGenericRepository<Rider>
{
    Task<Rider?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Rider>> GetAvailableRidersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Rider>> GetAllDetailedAsync(CancellationToken cancellationToken = default);
}
