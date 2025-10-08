namespace PaymentsService.Domain.ValueObjects;

public readonly record struct Percentage
{
    public decimal Fraction { get; }

    private Percentage(decimal fraction)
    {
        Fraction = decimal.Round(fraction, 4, MidpointRounding.AwayFromZero);
    }

    public static Percentage FromFraction(decimal fraction)
    {
        if (fraction is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Percentage must be between 0 and 1.");
        }

        return new Percentage(fraction);
    }
}
