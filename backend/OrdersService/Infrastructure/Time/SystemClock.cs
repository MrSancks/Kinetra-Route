using OrdersService.Application.Abstractions;

namespace OrdersService.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
