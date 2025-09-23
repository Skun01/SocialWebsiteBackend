using System;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(Guid recipientId, Guid triggerId, NotificationType type, Guid targetId);
    Task<List<NotificationResponse>> GetUserNotificationsAsync(Guid userId);
    Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
}
