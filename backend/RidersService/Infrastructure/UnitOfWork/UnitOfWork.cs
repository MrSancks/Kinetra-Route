using RidersService.Infrastructure.Data;
using RidersService.Infrastructure.Repositories;

namespace RidersService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly RidersDbContext _context;
    private readonly Lazy<IRiderRepository> _ridersRepository;

    public UnitOfWork(RidersDbContext context)
    {
        _context = context;
        _ridersRepository = new Lazy<IRiderRepository>(() => new RiderRepository(context));
    }

    public IRiderRepository Riders => _ridersRepository.Value;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}
