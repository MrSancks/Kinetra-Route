namespace PaymentsService.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
