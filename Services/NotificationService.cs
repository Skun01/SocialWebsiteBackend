using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Entities;
using SocialWebsite.Hubs;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IUserRepository _userRepo;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IHubContext<NotificationHub> hubContext)
    {
        _notificationRepo = notificationRepository;
        _userRepo = userRepository;
        _hubContext = hubContext;
    }

    public async Task<Result> CreateNotificationAsync(Guid recipientId, Guid triggerId, NotificationType type, Guid targetId)
    {
        if (recipientId == triggerId)
        {
            return Result.Failure(new Error("400", "Trigger user and recipient user are 同じ です"));
        }

        var notification = new Notification
        {
            RecipientUserId = recipientId,
            TriggeredByUserId = triggerId,
            Type = type,
            Link = $"/posts/{targetId}"
        };

        await _notificationRepo.AddAsync(notification);

        //USING SIGNALR
        
        // Lấy thông tin đầy đủ của người gây ra hành động để tạo DTO.
        // Client cần biết ai đã like/comment bài viết của họ.
        var triggeredByUser = await _userRepo.GetByIdAsync(triggerId);

        if (triggeredByUser != null)
        {
            // Tạo DTO (NotificationResponse) giống hệt như DTO bạn trả về trong API.
            // Điều này đảm bảo dữ liệu nhất quán ở mọi nơi.
            var notificationDto = new NotificationResponse(
                notification.Id,
                triggeredByUser.ToResponse(),
                notification.Type,
                GenerateMessage(notification.Type, triggeredByUser.Username),
                notification.Link,
                notification.IsRead,
                notification.CreatedAt
            );

            // Gửi DTO này tới group riêng của người nhận.
            await _hubContext.Clients
                .Group(recipientId.ToString()) // Chọn đúng kênh của người nhận
                .SendAsync("ReceiveNotification", notificationDto); // Gửi sự kiện "ReceiveNotification" với dữ liệu là DTO
        }

        return Result.Success();
    }

    public async Task<Result> DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepo.GetNotificationByIdAndUserIdAsync(notificationId, userId);

        if (notification == null)
        {
            return Result.Success();
        }

        await _notificationRepo.DeleteAsync(notification);
        return Result.Success();
    }

    public async Task<Result<UnreadCountResponse>> GetUnreadNotificationCountAsync(Guid userId)
    {
        var count = await _notificationRepo.GetUnreadNotificationCountAsync(userId);

        return Result.Success(new UnreadCountResponse(count));
    }

    public async Task<Result<CursorList<NotificationResponse>>> GetUserNotificationsAsync(Guid userId, NotificationQueryParameters query)
    {
        var notifications = await _notificationRepo.GetUserNotificationsAsync(userId, query);
        return Result.Success(notifications);
    }

    public async Task<Result<int>> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        int num = await _notificationRepo.MarkAllNotificationsAsReadAsync(userId);
        return Result.Success(num);
    }

    public async Task<Result> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepo.GetNotificationByIdAndUserIdAsync(notificationId, userId);

        if (notification == null || notification.IsRead)
            return Result.Failure(new Error("Notification.Mark", "Mark notification error"));

        notification.IsRead = true;
        await _notificationRepo.UpdateAsync(notification);
        return Result.Success();
    }

    private static string GenerateMessage(NotificationType type, string userName)
    {
        return type switch
        {
            NotificationType.NewLikeOnPost => $"{userName} đã thích bài viết của bạn.",
            NotificationType.NewCommentOnPost => $"{userName} đã bình luận về bài viết của bạn.",
            NotificationType.NewPostCreated => $"{userName} đã đăng một bài viết mới",
            NotificationType.NewFriendRequest => $"{userName} đã gửi cho bạn lời mời kết bạn.",
            NotificationType.FriendRequestAccepted => $"{userName} đã chấp nhận lời mời kết bạn của bạn.",
            _ => "Bạn có một thông báo mới."
        };
    }
    
}
