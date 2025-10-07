namespace RidersService.Application.DTOs;

public record DeliveryHistoryDto(
    Guid Id,
    string OrderId,
    DateTime DeliveredAt,
    string? Notes
);
