using System;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<Notification?> GetNotificationByIdAndUserIdAsync(Guid notificationId, Guid userId);
    Task<int> GetUnreadNotificationCountAsync(Guid userId);
    Task<CursorList<NotificationResponse>> GetUserNotificationsAsync(Guid userId, NotificationQueryParameters query);
    Task<int> MarkAllNotificationsAsReadAsync(Guid userId);
}