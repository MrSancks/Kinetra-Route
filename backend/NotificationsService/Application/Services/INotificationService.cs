using NotificationsService.Application.DTOs;
using NotificationsService.Application.Requests;

namespace NotificationsService.Application.Services;

public interface INotificationService
{
    Task<IReadOnlyCollection<NotificationDto>> NotifyNewOrderAsync(SendNewOrderNotificationRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<NotificationDto>> NotifyDeliveryStatusAsync(SendDeliveryStatusNotificationRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<NotificationDto>> NotifyAlertAsync(SendAlertNotificationRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<NotificationDto>> GetPendingAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxMessageDto>> GetOutboxPendingAsync(CancellationToken cancellationToken);
}
