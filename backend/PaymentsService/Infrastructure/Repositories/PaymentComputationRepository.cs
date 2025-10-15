using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentsService.Application.Abstractions;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Infrastructure.Data.Models;

namespace PaymentsService.Infrastructure.Repositories;

public sealed class PaymentComputationRepository : IPaymentComputationRepository
{
    private readonly PaymentsDbContext _context;

    public PaymentComputationRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(string computationType, string requestPayload, string responsePayload, CancellationToken cancellationToken)
    {
        var record = new PaymentComputationRecord
        {
            Id = Guid.NewGuid(),
            ComputationType = computationType,
            RequestJson = requestPayload,
            ResponseJson = responsePayload,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.Computations.AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
