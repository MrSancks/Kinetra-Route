namespace RidersService.Application.DTOs;

public record VehicleDto(
    Guid Id,
    string Type,
    string Brand,
    string Model,
    string PlateNumber,
    string Color,
    DateTime? InsuranceExpiration
);
