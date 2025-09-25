using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class NotificationService : INotificationService
{
    private readonly SocialWebsiteContext _context;
    public NotificationService(SocialWebsiteContext context)
    {
        _context = context;
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

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);

        if (notification == null)
        {
            return Result.Success();
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<UnreadCountResponse>> GetUnreadNotificationCountAsync(Guid userId)
    {
        var count = await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);

        return Result.Success(new UnreadCountResponse(count));
    }

    public async Task<Result<CursorList<NotificationResponse>>> GetUserNotificationsAsync(Guid userId, NotificationQueryParameters query)
    {
        var baseQuery = _context.Notifications.AsNoTracking();
        baseQuery = baseQuery.Where(n => n.RecipientUserId == userId)
                             .Include(n => n.TriggeredByUser)
                             .OrderByDescending(n => n.CreatedAt);
        var decodedCursor = CursorHelper.DecodeCursor(query.Cursor);
        if (decodedCursor.HasValue)
        {
            var (cursorCreatedAt, cursorId) = decodedCursor.Value;
            baseQuery = baseQuery.Where(p =>
                p.CreatedAt < cursorCreatedAt ||
                (p.CreatedAt == cursorCreatedAt && p.Id.CompareTo(cursorId) < 0)
            );
        }
        var notifications = await baseQuery
            .Include(n => n.TriggeredByUser)
            .Take(query.PageSize + 1)
            .Select(n => new NotificationResponse(
                n.Id,
                n.TriggeredByUser.ToResponse(),
                n.Type,
                GenerateMessage(n.Type, n.TriggeredByUser.Username),
                n.Link,
                n.IsRead,
                n.CreatedAt
            )).ToListAsync();

        bool hasNextPage = notifications.Count > query.PageSize;
        string? nextCursor = null;
        if (hasNextPage)
        {
            notifications.RemoveAt(query.PageSize);
            var lastItem = notifications.Last();
            if (lastItem != null)
                nextCursor = CursorHelper.EncodeCursor(lastItem.CreatedAt, lastItem.Id);
        }

        return Result.Success(new CursorList<NotificationResponse>(notifications, nextCursor, hasNextPage));
    }

    public async Task<Result<int>> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        int num = await _context.Notifications
           .Where(n => n.RecipientUserId == userId && !n.IsRead)
           .ExecuteUpdateAsync(updates => updates.SetProperty(n => n.IsRead, true));

        return Result.Success(num);
    }

    public async Task<Result> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);

        if (notification == null || notification.IsRead)
            return Result.Failure(new Error("Notification.Mark", "Mark notification error"));

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    private static string GenerateMessage(NotificationType type, string userName)
    {
        return type switch
        {
            NotificationType.NewLikeOnPost => $"{userName} đã thích bài viết của bạn.",
            NotificationType.NewCommentOnPost => $"{userName} đã bình luận về bài viết của bạn.",
            _ => "Bạn có một thông báo mới."
        };
    }
    
}
