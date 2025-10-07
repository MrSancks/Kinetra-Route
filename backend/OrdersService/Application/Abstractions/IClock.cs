namespace OrdersService.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
