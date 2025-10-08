using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NotificationsService.Application.Requests;
using NotificationsService.Application.Services;

namespace NotificationsService.Presentation;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications");

        group.MapPost("/orders/new", async (SendNewOrderNotificationRequest request, INotificationService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var notifications = await service.NotifyNewOrderAsync(request, cancellationToken);
            return Results.Accepted(value: notifications);
        }).WithName("NotifyNewOrder");

        group.MapPost("/orders/status", async (SendDeliveryStatusNotificationRequest request, INotificationService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var notifications = await service.NotifyDeliveryStatusAsync(request, cancellationToken);
            return Results.Accepted(value: notifications);
        }).WithName("NotifyDeliveryStatus");

        group.MapPost("/orders/alerts", async (SendAlertNotificationRequest request, INotificationService service, CancellationToken cancellationToken) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var notifications = await service.NotifyAlertAsync(request, cancellationToken);
            return Results.Accepted(value: notifications);
        }).WithName("NotifyAlert");

        group.MapGet("/pending", async (INotificationService service, CancellationToken cancellationToken) =>
        {
            var notifications = await service.GetPendingAsync(cancellationToken);
            return Results.Ok(notifications);
        }).WithName("GetPendingNotifications");

        group.MapGet("/outbox/pending", async (INotificationService service, CancellationToken cancellationToken) =>
        {
            var messages = await service.GetOutboxPendingAsync(cancellationToken);
            return Results.Ok(messages);
        }).WithName("GetPendingOutboxMessages");
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
