namespace PaymentsService.Domain.ValueObjects;

public readonly record struct Distance
{
    public decimal Kilometers { get; }

    private Distance(decimal kilometers)
    {
        Kilometers = Math.Round(kilometers, 3, MidpointRounding.AwayFromZero);
    }

    public static Distance FromKilometers(decimal kilometers)
    {
        if (kilometers < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kilometers), "Distance cannot be negative.");
        }

        return new Distance(kilometers);
    }

    public bool IsLongerThan(Distance other) => Kilometers >= other.Kilometers;

    public decimal ToDecimal() => Kilometers;

    public override string ToString() => Kilometers.ToString("0.###");
}
