using RidersService.Infrastructure.Repositories;

namespace RidersService.Infrastructure.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRiderRepository Riders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
