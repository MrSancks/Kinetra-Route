using System.ComponentModel.DataAnnotations;
using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.Requests;

public record SendDeliveryStatusNotificationRequest(
    [property: Required] string OrderId,
    [property: Required] string Status,
    string? RiderId,
    string? RestaurantId,
    NotificationChannel PreferredChannel = NotificationChannel.Push
);
