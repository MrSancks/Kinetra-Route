using System.ComponentModel.DataAnnotations;

namespace ReportsService.Contracts.Requests;

public sealed class RecordOrderCompletionRequest
{
    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public string RiderId { get; set; } = string.Empty;

    [Required]
    public DateTime CompletedAt { get; set; }
        = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal OrderTotal { get; set; }
        = 0m;

    [Range(0, double.MaxValue)]
    public decimal PlatformFee { get; set; }
        = 0m;

    public bool DeliveredOnTime { get; set; }
        = true;

    [Range(0, double.MaxValue)]
    public double? DeliveryDurationMinutes { get; set; }
        = null;
}
