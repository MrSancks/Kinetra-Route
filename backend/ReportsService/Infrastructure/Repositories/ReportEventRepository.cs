using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportsService.Application.Abstractions;
using ReportsService.Domain.Events;
using ReportsService.Infrastructure.Data;
using ReportsService.Infrastructure.Data.Models;

namespace ReportsService.Infrastructure.Repositories;

public sealed class ReportEventRepository : IReportEventRepository
{
    private readonly ReportsDbContext _context;

    public ReportEventRepository(ReportsDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(OrderCompletedEvent @event, CancellationToken cancellationToken)
    {
        var record = new OrderCompletedEventRecord
        {
            Id = Guid.NewGuid(),
            OrderId = @event.OrderId,
            RiderId = @event.RiderId,
            CompletedAt = @event.CompletedAt,
            OrderTotal = @event.OrderTotal,
            PlatformFee = @event.PlatformFee,
            DeliveredOnTime = @event.DeliveredOnTime,
            DeliveryDurationMinutes = @event.DeliveryDurationMinutes
        };

        await _context.OrderCompletedEvents.AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<OrderCompletedEvent>> LoadAllAsync(CancellationToken cancellationToken)
    {
        var records = await _context.OrderCompletedEvents
            .AsNoTracking()
            .OrderBy(record => record.CompletedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return records
            .Select(record => new OrderCompletedEvent(
                record.OrderId,
                record.RiderId,
                record.CompletedAt,
                record.OrderTotal,
                record.PlatformFee,
                record.DeliveredOnTime,
                record.DeliveryDurationMinutes))
            .ToList();
    }
}
