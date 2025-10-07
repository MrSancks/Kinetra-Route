using System;
using System.Collections.Generic;
using System.Linq;
using RidersService.Application.DTOs;
using RidersService.Domain.Entities;
using RidersService.Domain.Enums;
using RidersService.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace RidersService.Application.Services;

public interface IRiderService
{
    Task<IReadOnlyCollection<RiderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RiderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RiderDto> CreateAsync(CreateRiderRequest request, CancellationToken cancellationToken = default);
    Task<RiderDto?> UpdateAsync(Guid id, UpdateRiderRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RiderDto?> UpdateAvailabilityAsync(Guid id, AvailabilityStatus availability, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RiderDto>> GetAvailableAsync(CancellationToken cancellationToken = default);
    Task<DeliveryHistoryDto?> AddDeliveryAsync(Guid riderId, CreateDeliveryRecordRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DeliveryHistoryDto>> GetDeliveryHistoryAsync(Guid riderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RiderDocumentDto>?> ValidateDocumentsAsync(Guid riderId, CancellationToken cancellationToken = default);
}

public class RiderService : IRiderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentValidationService _documentValidationService;
    private readonly ILogger<RiderService> _logger;

    public RiderService(IUnitOfWork unitOfWork, IDocumentValidationService documentValidationService, ILogger<RiderService> logger)
    {
        _unitOfWork = unitOfWork;
        _documentValidationService = documentValidationService;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<RiderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var riders = await _unitOfWork.Riders.GetAllDetailedAsync(cancellationToken);
        return riders.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<RiderDto>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        var riders = await _unitOfWork.Riders.GetAvailableRidersAsync(cancellationToken);
        return riders.Select(MapToDto).ToList();
    }

    public async Task<RiderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(id, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Rider with id {RiderId} was not found", id);
            return null;
        }

        return MapToDto(rider);
    }

    public async Task<RiderDto> CreateAsync(CreateRiderRequest request, CancellationToken cancellationToken = default)
    {
        var rider = new Rider
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Availability = AvailabilityStatus.Offline,
            Vehicle = request.Vehicle is null ? null : new Vehicle
            {
                Type = request.Vehicle.Type,
                Brand = request.Vehicle.Brand,
                Model = request.Vehicle.Model,
                PlateNumber = request.Vehicle.PlateNumber,
                Color = request.Vehicle.Color,
                InsuranceExpiration = request.Vehicle.InsuranceExpiration
            },
            Documents = request.Documents.Select(d => new RiderDocument
            {
                Type = d.Type,
                Number = d.Number,
                IssueDate = d.IssueDate,
                ExpirationDate = d.ExpirationDate,
                IsVerified = false
            }).ToList()
        };

        await _unitOfWork.Riders.AddAsync(rider, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _documentValidationService.ValidateDocuments(rider.Documents);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(rider);
    }

    public async Task<RiderDto?> UpdateAsync(Guid id, UpdateRiderRequest request, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(id, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Attempted to update rider {RiderId} but it does not exist", id);
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            rider.FirstName = request.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            rider.LastName = request.LastName;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            rider.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            rider.PhoneNumber = request.PhoneNumber;
        }

        if (request.Vehicle is not null)
        {
            if (rider.Vehicle is null)
            {
                rider.Vehicle = new Vehicle { RiderId = rider.Id };
            }

            rider.Vehicle.Type = request.Vehicle.Type ?? rider.Vehicle.Type;
            rider.Vehicle.Brand = request.Vehicle.Brand ?? rider.Vehicle.Brand;
            rider.Vehicle.Model = request.Vehicle.Model ?? rider.Vehicle.Model;
            rider.Vehicle.PlateNumber = request.Vehicle.PlateNumber ?? rider.Vehicle.PlateNumber;
            rider.Vehicle.Color = request.Vehicle.Color ?? rider.Vehicle.Color;
            rider.Vehicle.InsuranceExpiration = request.Vehicle.InsuranceExpiration ?? rider.Vehicle.InsuranceExpiration;
        }

        if (request.Documents is not null)
        {
            var documentsById = rider.Documents.ToDictionary(d => d.Id);

            foreach (var documentUpdate in request.Documents)
            {
                if (documentUpdate.Id.HasValue && documentsById.TryGetValue(documentUpdate.Id.Value, out var existingDocument))
                {
                    existingDocument.Type = documentUpdate.Type;
                    existingDocument.Number = documentUpdate.Number;
                    existingDocument.IssueDate = documentUpdate.IssueDate;
                    existingDocument.ExpirationDate = documentUpdate.ExpirationDate;
                    existingDocument.IsVerified = documentUpdate.IsVerified;
                }
                else
                {
                    rider.Documents.Add(new RiderDocument
                    {
                        Type = documentUpdate.Type,
                        Number = documentUpdate.Number,
                        IssueDate = documentUpdate.IssueDate,
                        ExpirationDate = documentUpdate.ExpirationDate,
                        IsVerified = documentUpdate.IsVerified
                    });
                }
            }
        }

        rider.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Riders.Update(rider);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _documentValidationService.ValidateDocuments(rider.Documents);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(rider);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(id, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Attempted to delete rider {RiderId} but it does not exist", id);
            return false;
        }

        _unitOfWork.Riders.Remove(rider);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<RiderDto?> UpdateAvailabilityAsync(Guid id, AvailabilityStatus availability, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(id, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Attempted to change availability for rider {RiderId} but it does not exist", id);
            return null;
        }

        rider.Availability = availability;
        rider.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Riders.Update(rider);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(rider);
    }

    public async Task<DeliveryHistoryDto?> AddDeliveryAsync(Guid riderId, CreateDeliveryRecordRequest request, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(riderId, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Attempted to add a delivery for rider {RiderId} but it does not exist", riderId);
            return null;
        }

        var delivery = new DeliveryHistory
        {
            RiderId = rider.Id,
            OrderId = request.OrderId,
            DeliveredAt = request.DeliveredAt ?? DateTime.UtcNow,
            Notes = request.Notes
        };

        rider.Deliveries.Add(delivery);
        rider.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Riders.Update(rider);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeliveryHistoryDto(delivery.Id, delivery.OrderId, delivery.DeliveredAt, delivery.Notes);
    }

    public async Task<IReadOnlyCollection<DeliveryHistoryDto>> GetDeliveryHistoryAsync(Guid riderId, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(riderId, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Requested delivery history for rider {RiderId} but it does not exist", riderId);
            return Array.Empty<DeliveryHistoryDto>();
        }

        return rider.Deliveries
            .OrderByDescending(d => d.DeliveredAt)
            .Select(d => new DeliveryHistoryDto(d.Id, d.OrderId, d.DeliveredAt, d.Notes))
            .ToList();
    }

    public async Task<IReadOnlyCollection<RiderDocumentDto>?> ValidateDocumentsAsync(Guid riderId, CancellationToken cancellationToken = default)
    {
        var rider = await _unitOfWork.Riders.GetDetailedByIdAsync(riderId, cancellationToken);
        if (rider is null)
        {
            _logger.LogWarning("Attempted to validate documents for rider {RiderId} but it does not exist", riderId);
            return null;
        }

        var validated = _documentValidationService.ValidateDocuments(rider.Documents);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return validated;
    }

    private RiderDto MapToDto(Rider rider)
    {
        return new RiderDto(
            rider.Id,
            rider.FirstName,
            rider.LastName,
            rider.Email,
            rider.PhoneNumber,
            rider.Availability,
            rider.CreatedAt,
            rider.UpdatedAt,
            rider.Vehicle is null
                ? null
                : new VehicleDto(
                    rider.Vehicle.Id,
                    rider.Vehicle.Type,
                    rider.Vehicle.Brand,
                    rider.Vehicle.Model,
                    rider.Vehicle.PlateNumber,
                    rider.Vehicle.Color,
                    rider.Vehicle.InsuranceExpiration),
            _documentValidationService.ValidateDocuments(rider.Documents),
            rider.Deliveries
                .OrderByDescending(d => d.DeliveredAt)
                .Select(d => new DeliveryHistoryDto(d.Id, d.OrderId, d.DeliveredAt, d.Notes))
                .ToList());
    }
}
