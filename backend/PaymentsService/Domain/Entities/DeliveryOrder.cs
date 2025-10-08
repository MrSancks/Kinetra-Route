using PaymentsService.Domain.ValueObjects;

namespace PaymentsService.Domain.Entities;

public sealed class DeliveryOrder
{
    public string OrderId { get; }
    public string RiderId { get; }
    public Distance Distance { get; }
    public Money BaseFee { get; }
    public DateTime CompletedAt { get; }

    private DeliveryOrder(string orderId, string riderId, Distance distance, Money baseFee, DateTime completedAt)
    {
        OrderId = orderId;
        RiderId = riderId;
        Distance = distance;
        BaseFee = baseFee;
        CompletedAt = completedAt;
    }

    public static DeliveryOrder Create(string orderId, string riderId, Distance distance, Money baseFee, DateTime completedAt)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("Order identifier cannot be empty.", nameof(orderId));
        }

        if (string.IsNullOrWhiteSpace(riderId))
        {
            throw new ArgumentException("Rider identifier cannot be empty.", nameof(riderId));
        }

        return new DeliveryOrder(orderId, riderId, distance, baseFee, completedAt);
    }
}
