namespace OrdersService.Domain.Entities;

public enum OrderStatus
{
    Created,
    Assigned,
    EnRoute,
    Delivered,
    Cancelled
}
