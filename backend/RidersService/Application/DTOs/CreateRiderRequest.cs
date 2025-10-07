using System.ComponentModel.DataAnnotations;
using RidersService.Domain.Enums;

namespace RidersService.Application.DTOs;

public class CreateRiderRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public CreateVehicleRequest? Vehicle { get; set; }

    public ICollection<CreateRiderDocumentRequest> Documents { get; set; } = new List<CreateRiderDocumentRequest>();
}

public class CreateVehicleRequest
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string PlateNumber { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public DateTime? InsuranceExpiration { get; set; }
}

public class CreateRiderDocumentRequest
{
    [Required]
    public DocumentType Type { get; set; }

    [Required]
    [MaxLength(50)]
    public string Number { get; set; } = string.Empty;

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }
}
