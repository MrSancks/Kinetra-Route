using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions;

public interface IAccessLogRepository
{
    Task AddAsync(AccessLog log, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AccessLog>> GetAsync(DateTime? from, DateTime? to, Guid? userId, CancellationToken cancellationToken);
}
