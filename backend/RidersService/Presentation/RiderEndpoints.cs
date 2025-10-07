using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RidersService.Application.DTOs;
using RidersService.Application.Services;

namespace RidersService.Presentation;

public static class RiderEndpoints
{
    public static void MapRiderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/riders")
            .WithTags("Riders");

        group.MapGet("/", async (IRiderService service, CancellationToken cancellationToken) =>
        {
            var riders = await service.GetAllAsync(cancellationToken);
            return Results.Ok(riders);
        }).WithName("GetRiders");

        group.MapGet("/available", async (IRiderService service, CancellationToken cancellationToken) =>
        {
            var riders = await service.GetAvailableAsync(cancellationToken);
            return Results.Ok(riders);
        }).WithName("GetAvailableRiders");

        group.MapGet("/{id:guid}", async (Guid id, IRiderService service, CancellationToken cancellationToken) =>
        {
            var rider = await service.GetByIdAsync(id, cancellationToken);
            return rider is not null ? Results.Ok(rider) : Results.NotFound();
        }).WithName("GetRiderById");

        group.MapPost("/", async (CreateRiderRequest request, IRiderService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var rider = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/riders/{rider.Id}", rider);
        }).WithName("CreateRider");

        group.MapPut("/{id:guid}", async (Guid id, UpdateRiderRequest request, IRiderService service, CancellationToken cancellationToken) =>
        {
            var rider = await service.UpdateAsync(id, request, cancellationToken);
            return rider is not null ? Results.Ok(rider) : Results.NotFound();
        }).WithName("UpdateRider");

        group.MapDelete("/{id:guid}", async (Guid id, IRiderService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteRider");

        group.MapPatch("/{id:guid}/availability", async (Guid id, UpdateAvailabilityRequest request, IRiderService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var rider = await service.UpdateAvailabilityAsync(id, request.Availability, cancellationToken);
            return rider is not null ? Results.Ok(rider) : Results.NotFound();
        }).WithName("UpdateRiderAvailability");

        group.MapGet("/{id:guid}/deliveries", async (Guid id, IRiderService service, CancellationToken cancellationToken) =>
        {
            var deliveries = await service.GetDeliveryHistoryAsync(id, cancellationToken);
            return Results.Ok(deliveries);
        }).WithName("GetRiderDeliveries");

        group.MapPost("/{id:guid}/deliveries", async (Guid id, CreateDeliveryRecordRequest request, IRiderService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var delivery = await service.AddDeliveryAsync(id, request, cancellationToken);
            return delivery is not null ? Results.Ok(delivery) : Results.NotFound();
        }).WithName("AddRiderDelivery");

        group.MapPost("/{id:guid}/validate-documents", async (Guid id, IRiderService service, CancellationToken cancellationToken) =>
        {
            var documents = await service.ValidateDocumentsAsync(id, cancellationToken);
            return documents is not null ? Results.Ok(documents) : Results.NotFound();
        }).WithName("ValidateRiderDocuments");
    }

    private static class MiniValidator
    {
        public static bool TryValidate<T>(T model, out Dictionary<string, string[]> errors)
        {
            var validationContext = new ValidationContext(model!, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model!, validationContext, validationResults, true);

            errors = validationResults
                .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ErrorMessage ?? string.Empty).ToArray());

            return isValid;
        }
    }
}
