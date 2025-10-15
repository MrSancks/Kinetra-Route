using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using OrdersService.Application.Abstractions;
using OrdersService.Domain.Entities;
using OrdersService.Infrastructure.Data;
using OrdersService.Infrastructure.Data.Models;

namespace OrdersService.Infrastructure.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        var exists = await _context.Orders
            .AsNoTracking()
            .AnyAsync(o => o.Id == order.Id, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            throw new InvalidOperationException($"The order {order.Id} already exists");
        }

        var record = new OrderRecord
        {
            Id = order.Id,
            PayloadJson = Serialize(order),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt ?? order.CreatedAt
        };

        await _context.Orders.AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            .ConfigureAwait(false);

        return record is null ? null : Deserialize(record.PayloadJson);
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        var records = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return records
            .Select(record => Deserialize(record.PayloadJson))
            .Where(order => order is not null)
            .Select(order => order!)
            .ToArray();
    }

    public async Task<Order?> UpdateAsync(Guid id, Func<Order, Order> update, CancellationToken cancellationToken)
    {
        var record = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            return null;
        }

        var current = Deserialize(record.PayloadJson);
        if (current is null)
        {
            return null;
        }

        var updated = update(current);

        record.PayloadJson = Serialize(updated);
        record.UpdatedAt = updated.UpdatedAt ?? updated.CreatedAt;

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return updated;
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _context.Orders.FindAsync([id], cancellationToken).ConfigureAwait(false);
        if (record is null)
        {
            return false;
        }

        _context.Orders.Remove(record);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private static string Serialize(Order order) =>
        JsonSerializer.Serialize(order, SerializerOptions);

    private static Order? Deserialize(string payload) =>
        JsonSerializer.Deserialize<Order>(payload, SerializerOptions);
}
