using System.ComponentModel.DataAnnotations;
using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.Requests;

public record SendNewOrderNotificationRequest(
    [property: Required] string OrderId,
    [property: Required] string RestaurantId,
    [property: Required] string RestaurantContact,
    string? RiderId,
    string? RiderContact,
    [property: Required] string DeliveryAddress,
    NotificationChannel PreferredChannel = NotificationChannel.Push
);
