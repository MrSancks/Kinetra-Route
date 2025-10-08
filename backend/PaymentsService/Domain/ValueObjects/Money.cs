namespace PaymentsService.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static Money Zero => new(0m);

    public static Money From(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        return new Money(amount);
    }

    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other)
    {
        var result = Amount - other.Amount;
        if (result < 0)
        {
            result = 0;
        }

        return new Money(result);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), "Multiplication factor cannot be negative.");
        }

        return new Money(Amount * factor);
    }

    public decimal ToDecimal() => Amount;

    public override string ToString() => Amount.ToString("0.00");
}
