using System.ComponentModel.DataAnnotations;

namespace RidersService.Application.DTOs;

public class CreateDeliveryRecordRequest
{
    [Required]
    public string OrderId { get; set; } = string.Empty;

    public DateTime? DeliveredAt { get; set; }

    public string? Notes { get; set; }
}
