using System.ComponentModel.DataAnnotations;
using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.Requests;

public record SendAlertNotificationRequest(
    [property: Required] string OrderId,
    [property: Required] AlertType AlertType,
    [property: Required] string Message,
    string? RiderId,
    string? RestaurantId,
    NotificationChannel PreferredChannel = NotificationChannel.Push
);
