using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.DTOs;
using NotificationsService.Application.Requests;
using NotificationsService.Application.Services;
using NotificationsService.Domain.Entities;
using NotificationsService.Domain.Enums;
using NotificationsService.Infrastructure.Data;

namespace NotificationsService.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationsDbContext _dbContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(NotificationsDbContext dbContext, ILogger<NotificationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<NotificationDto>> NotifyNewOrderAsync(SendNewOrderNotificationRequest request, CancellationToken cancellationToken)
    {
        var notifications = new List<Notification>
        {
            CreateNotification(
                recipientId: request.RestaurantId,
                RecipientType.Restaurant,
                request.PreferredChannel,
                NotificationType.NewOrder,
                $"Nuevo pedido #{request.OrderId}",
                $"Se generó un nuevo pedido para entrega en {request.DeliveryAddress}. Contacto: {request.RestaurantContact}"
            )
        };

        if (!string.IsNullOrWhiteSpace(request.RiderId))
        {
            notifications.Add(CreateNotification(
                recipientId: request.RiderId!,
                RecipientType.Rider,
                request.PreferredChannel,
                NotificationType.NewOrder,
                $"Nuevo pedido asignado #{request.OrderId}",
                $"Debes recoger el pedido en {request.RestaurantContact} y entregarlo en {request.DeliveryAddress}."
            ));
        }

        await PersistNotificationsAsync(notifications, "notifications.new-order", cancellationToken);
        return notifications.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<NotificationDto>> NotifyDeliveryStatusAsync(SendDeliveryStatusNotificationRequest request, CancellationToken cancellationToken)
    {
        var notifications = new List<Notification>();

        if (!string.IsNullOrWhiteSpace(request.RiderId))
        {
            notifications.Add(CreateNotification(
                recipientId: request.RiderId!,
                RecipientType.Rider,
                request.PreferredChannel,
                NotificationType.DeliveryStatus,
                $"Estado actualizado del pedido #{request.OrderId}",
                $"El estado del pedido ahora es: {request.Status}."
            ));
        }

        if (!string.IsNullOrWhiteSpace(request.RestaurantId))
        {
            notifications.Add(CreateNotification(
                recipientId: request.RestaurantId!,
                RecipientType.Restaurant,
                request.PreferredChannel,
                NotificationType.DeliveryStatus,
                $"Estado del pedido #{request.OrderId}",
                $"El pedido cambió al estado: {request.Status}."
            ));
        }

        await PersistNotificationsAsync(notifications, "notifications.delivery-status", cancellationToken);
        return notifications.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<NotificationDto>> NotifyAlertAsync(SendAlertNotificationRequest request, CancellationToken cancellationToken)
    {
        var notifications = new List<Notification>();

        if (!string.IsNullOrWhiteSpace(request.RiderId))
        {
            notifications.Add(CreateNotification(
                recipientId: request.RiderId!,
                RecipientType.Rider,
                request.PreferredChannel,
                NotificationType.Alert,
                BuildAlertTitle(request.AlertType, request.OrderId),
                request.Message
            ));
        }

        if (!string.IsNullOrWhiteSpace(request.RestaurantId))
        {
            notifications.Add(CreateNotification(
                recipientId: request.RestaurantId!,
                RecipientType.Restaurant,
                request.PreferredChannel,
                NotificationType.Alert,
                BuildAlertTitle(request.AlertType, request.OrderId),
                request.Message
            ));
        }

        await PersistNotificationsAsync(notifications, "notifications.alert", cancellationToken);
        return notifications.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<NotificationDto>> GetPendingAsync(CancellationToken cancellationToken)
    {
        var notifications = await _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<OutboxMessageDto>> GetOutboxPendingAsync(CancellationToken cancellationToken)
    {
        var outbox = await _dbContext.OutboxMessages
            .AsNoTracking()
            .Where(o => o.ProcessedOn == null)
            .OrderBy(o => o.OccurredOn)
            .ToListAsync(cancellationToken);

        return outbox.Select(message => new OutboxMessageDto(
            message.Id,
            message.EventType,
            message.Payload,
            message.OccurredOn,
            message.ProcessedOn,
            message.Error
        )).ToList();
    }

    private static Notification CreateNotification(
        string recipientId,
        RecipientType recipientType,
        NotificationChannel channel,
        NotificationType type,
        string title,
        string message) => new()
        {
            RecipientId = recipientId,
            RecipientType = recipientType,
            Channel = channel,
            Type = type,
            Title = title,
            Message = message
        };

    private static string BuildAlertTitle(AlertType alertType, string orderId) => alertType switch
    {
        AlertType.OrderCancelled => $"Pedido #{orderId} cancelado",
        AlertType.RouteChanged => $"Ruta actualizada para pedido #{orderId}",
        _ => $"Alerta para pedido #{orderId}"
    };

    private async Task PersistNotificationsAsync(IEnumerable<Notification> notifications, string eventType, CancellationToken cancellationToken)
    {
        if (!notifications.Any())
        {
            _logger.LogInformation("No se generaron notificaciones para el evento {EventType}", eventType);
            return;
        }

        var useTransaction = _dbContext.Database.IsRelational();
        IDbContextTransaction? transaction = null;

        if (useTransaction)
        {
            transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        foreach (var notification in notifications)
        {
            await _dbContext.Notifications.AddAsync(notification, cancellationToken);
            await _dbContext.OutboxMessages.AddAsync(new OutboxMessage
            {
                EventType = eventType,
                Payload = JsonSerializer.Serialize(new
                {
                    NotificationId = notification.Id,
                    notification.RecipientId,
                    RecipientType = notification.RecipientType.ToString(),
                    Channel = notification.Channel.ToString(),
                    Type = notification.Type.ToString(),
                    notification.Title,
                    notification.Message,
                    notification.CreatedAt
                })
            }, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    private static NotificationDto MapToDto(Notification notification) => new(
        notification.Id,
        notification.RecipientId,
        notification.RecipientType,
        notification.Channel,
        notification.Type,
        notification.Title,
        notification.Message,
        notification.Status,
        notification.CreatedAt,
        notification.SentAt);
}
