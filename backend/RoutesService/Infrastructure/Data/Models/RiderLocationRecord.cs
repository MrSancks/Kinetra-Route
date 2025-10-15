using System;

namespace RoutesService.Infrastructure.Data.Models;

public class RiderLocationRecord
{
    public string RiderId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
}
