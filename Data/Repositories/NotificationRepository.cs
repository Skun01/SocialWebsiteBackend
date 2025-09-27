using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly SocialWebsiteContext _context;

    public NotificationRepository(SocialWebsiteContext context)
    {
        _context = context;
    }

    public async Task<Notification> AddAsync(Notification entity)
    {
        _context.Notifications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Notification entity)
    {
        _context.Notifications.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Notification?> GetNotificationByIdAndUserIdAsync(Guid notificationId, Guid userId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientUserId == userId);
    }

    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);
    }

    public async Task<CursorList<NotificationResponse>> GetUserNotificationsAsync(Guid userId, NotificationQueryParameters query)
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

        return new CursorList<NotificationResponse>(notifications, nextCursor, hasNextPage);
    }

    public async Task<int> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        return await _context.Notifications
           .Where(n => n.RecipientUserId == userId && !n.IsRead)
           .ExecuteUpdateAsync(updates => updates.SetProperty(n => n.IsRead, true));
    }

    public async Task UpdateAsync(Notification entity)
    {
        _context.Notifications.Update(entity);
        await _context.SaveChangesAsync();
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