namespace PaymentsService.Application.Abstractions;

public interface IPaymentComputationRepository
{
    Task SaveAsync(string computationType, string requestPayload, string responsePayload, CancellationToken cancellationToken);
}
