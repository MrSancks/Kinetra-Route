using System.ComponentModel.DataAnnotations;
using RidersService.Domain.Enums;

namespace RidersService.Application.DTOs;

public class UpdateRiderRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public UpdateVehicleRequest? Vehicle { get; set; }

    public ICollection<UpdateRiderDocumentRequest>? Documents { get; set; }
}

public class UpdateVehicleRequest
{
    public string? Type { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? PlateNumber { get; set; }
    public string? Color { get; set; }
    public DateTime? InsuranceExpiration { get; set; }
}

public class UpdateRiderDocumentRequest
{
    public Guid? Id { get; set; }
    public DocumentType Type { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsVerified { get; set; }
}
