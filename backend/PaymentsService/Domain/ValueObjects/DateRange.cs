namespace PaymentsService.Domain.ValueObjects;

public sealed record DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    private DateRange(DateOnly start, DateOnly end)
    {
        Start = start;
        End = end;
    }

    public static DateRange Create(DateOnly start, DateOnly end)
    {
        if (end < start)
        {
            throw new ArgumentException("The end of the range must be greater than or equal to the start.", nameof(end));
        }

        return new DateRange(start, end);
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;
}
