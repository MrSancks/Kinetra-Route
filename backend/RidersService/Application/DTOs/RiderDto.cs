using RidersService.Domain.Enums;

namespace RidersService.Application.DTOs;

public record RiderDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    AvailabilityStatus Availability,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    VehicleDto? Vehicle,
    IReadOnlyCollection<RiderDocumentDto> Documents,
    IReadOnlyCollection<DeliveryHistoryDto> Deliveries
);
