using System;

namespace RoutesService.Infrastructure.Data.Models;

public class RoutePlanRecord
{
    public string DeliveryId { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}
