using System;

namespace ReportsService.Infrastructure.Data.Models;

public class OrderCompletedEventRecord
{
    public Guid Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string RiderId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal PlatformFee { get; set; }
    public bool DeliveredOnTime { get; set; }
    public double? DeliveryDurationMinutes { get; set; }
}
