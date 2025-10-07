using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using MediatR;

namespace AuthService.Application.Features.AccessLogs.Queries;

public class GetAccessLogsQueryHandler : IRequestHandler<GetAccessLogsQuery, IReadOnlyCollection<AccessLogDto>>
{
    private readonly IAccessLogRepository _accessLogRepository;

    public GetAccessLogsQueryHandler(IAccessLogRepository accessLogRepository)
    {
        _accessLogRepository = accessLogRepository;
    }

    public async Task<IReadOnlyCollection<AccessLogDto>> Handle(GetAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.GetAsync(request.From, request.To, request.UserId, cancellationToken);

        return logs
            .Select(log => new AccessLogDto(
                log.Id,
                log.UserId,
                log.User?.Email ?? string.Empty,
                log.User?.Role ?? Domain.Enums.Role.Rider,
                log.Action,
                log.OccurredAt,
                log.IpAddress))
            .ToList();
    }
}
