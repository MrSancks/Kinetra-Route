using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class AccessLogRepository : IAccessLogRepository
{
    private readonly AuthDbContext _context;

    public AccessLogRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AccessLog log, CancellationToken cancellationToken)
    {
        await _context.AccessLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AccessLog>> GetAsync(DateTime? from, DateTime? to, Guid? userId, CancellationToken cancellationToken)
    {
        var query = _context.AccessLogs
            .AsNoTracking()
            .Include(log => log.User)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(log => log.OccurredAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(log => log.OccurredAt <= to.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(log => log.UserId == userId.Value);
        }

        return await query
            .OrderByDescending(log => log.OccurredAt)
            .Take(250)
            .ToListAsync(cancellationToken);
    }
}
