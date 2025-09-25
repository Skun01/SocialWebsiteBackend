using System;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Services;

public interface INotificationService
{
    Task<Result> CreateNotificationAsync(Guid recipientId, Guid triggerId, NotificationType type, Guid targetId);
    Task<Result<CursorList<NotificationResponse>>> GetUserNotificationsAsync(Guid userId, NotificationQueryParameters query);
    Task<Result<UnreadCountResponse>> GetUnreadNotificationCountAsync(Guid userId);
    Task<Result> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    Task<Result<int>> MarkAllNotificationsAsReadAsync(Guid userId);
    Task<Result> DeleteNotificationAsync(Guid notificationId, Guid userId);
}
