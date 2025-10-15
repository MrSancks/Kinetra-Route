using System;

namespace PaymentsService.Infrastructure.Data.Models;

public class PaymentComputationRecord
{
    public Guid Id { get; set; }
    public string ComputationType { get; set; } = string.Empty;
    public string RequestJson { get; set; } = string.Empty;
    public string ResponseJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
