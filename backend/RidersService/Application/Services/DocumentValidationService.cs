using System.Collections.Generic;
using System.Linq;
using RidersService.Application.DTOs;
using RidersService.Domain.Entities;

namespace RidersService.Application.Services;

public interface IDocumentValidationService
{
    IReadOnlyCollection<RiderDocumentDto> ValidateDocuments(IEnumerable<RiderDocument> documents);
}

public class DocumentValidationService : IDocumentValidationService
{
    public IReadOnlyCollection<RiderDocumentDto> ValidateDocuments(IEnumerable<RiderDocument> documents)
    {
        return documents
            .Select(document =>
            {
                var isExpired = document.ExpirationDate.Date < DateTime.UtcNow.Date;
                document.IsVerified = !isExpired;

                return new RiderDocumentDto(
                    document.Id,
                    document.Type,
                    document.Number,
                    document.IssueDate,
                    document.ExpirationDate,
                    document.IsVerified,
                    isExpired);
            })
            .ToList();
    }
}
