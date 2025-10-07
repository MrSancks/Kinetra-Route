using RidersService.Domain.Enums;

namespace RidersService.Application.DTOs;

public record RiderDocumentDto(
    Guid Id,
    DocumentType Type,
    string Number,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsVerified,
    bool IsExpired
);
