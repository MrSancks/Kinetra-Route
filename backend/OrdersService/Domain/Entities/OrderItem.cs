namespace OrdersService.Domain.Entities;

public record OrderItem(string Sku, string Name, int Quantity, decimal UnitPrice);
