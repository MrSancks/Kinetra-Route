namespace PaymentsService.Domain.Entities;

public sealed class Rider
{
    public string RiderId { get; }
    public string Name { get; }

    private Rider(string riderId, string name)
    {
        RiderId = riderId;
        Name = name;
    }

    public static Rider Create(string riderId, string name)
    {
        if (string.IsNullOrWhiteSpace(riderId))
        {
            throw new ArgumentException("Rider identifier cannot be empty.", nameof(riderId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Rider name cannot be empty.", nameof(name));
        }

        return new Rider(riderId, name);
    }
}
