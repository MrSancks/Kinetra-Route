using System;

namespace OrdersService.Infrastructure.Data.Models;

public class OrderRecord
{
    public Guid Id { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
