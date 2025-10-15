using System;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using RoutesService.Application.Ports;
using RoutesService.Domain.Entities;
using RoutesService.Infrastructure.Data;
using RoutesService.Infrastructure.Data.Models;

namespace RoutesService.Infrastructure.Repositories;

public sealed class RoutePlanRepository : IRoutePlanRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RoutesDbContext _context;

    public RoutePlanRepository(RoutesDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(RoutePlan plan, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(plan.DeliveryId))
        {
            throw new InvalidOperationException("Route plan requires a delivery identifier.");
        }

        var payload = JsonSerializer.Serialize(plan, SerializerOptions);
        var record = await _context.RoutePlans
            .FirstOrDefaultAsync(r => r.DeliveryId == plan.DeliveryId, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            record = new RoutePlanRecord
            {
                DeliveryId = plan.DeliveryId!,
                PayloadJson = payload,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _context.RoutePlans.AddAsync(record, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            record.PayloadJson = payload;
            record.UpdatedAt = DateTimeOffset.UtcNow;
            _context.RoutePlans.Update(record);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<RoutePlan?> GetByDeliveryIdAsync(string deliveryId, CancellationToken cancellationToken)
    {
        var record = await _context.RoutePlans
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.DeliveryId == deliveryId, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<RoutePlan>(record.PayloadJson, SerializerOptions);
    }
}
