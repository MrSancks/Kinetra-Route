using AuthService.Application.DTOs;
using MediatR;

namespace AuthService.Application.Features.AccessLogs.Queries;

public record GetAccessLogsQuery(DateTime? From, DateTime? To, Guid? UserId) : IRequest<IReadOnlyCollection<AccessLogDto>>;
