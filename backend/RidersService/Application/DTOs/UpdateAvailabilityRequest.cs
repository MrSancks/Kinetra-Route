using System.ComponentModel.DataAnnotations;
using RidersService.Domain.Enums;

namespace RidersService.Application.DTOs;

public class UpdateAvailabilityRequest
{
    [Required]
    public AvailabilityStatus Availability { get; set; }
}
