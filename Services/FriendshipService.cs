using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Friendship;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class FriendshipService : IFriendshipService
{
    private readonly SocialWebsiteContext _context;
    private readonly INotificationService _notificationService;
    public FriendshipService(SocialWebsiteContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result> AcceptFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        var friendship = await _context.Friendships
           .FirstOrDefaultAsync(
               f => f.SenderId == senderId
               && f.ReceiverId == receiverId
               && f.Status == FriendshipStatus.Pending
           );

        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));

        friendship.Status = FriendshipStatus.Accepted;
        friendship.UpdatedAt = DateTime.UtcNow;

        _context.Friendships.Update(friendship);
        await _context.SaveChangesAsync();

        // Send notification to sender
        await _notificationService.CreateNotificationAsync(
            recipientId: senderId,
            triggerId: receiverId,
            type: NotificationType.FriendRequestAccepted,
            targetId: receiverId 
        );
        return Result.Success();
    }

    public async Task<Result> DeclineFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(
                f => f.SenderId == senderId
                && f.ReceiverId == receiverId
                && f.Status == FriendshipStatus.Pending
            );

        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetFriendsAsync(Guid userId)
    {
        var friends = await _context.Friendships
            .AsNoTracking()
            .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted)
            .Select(f => new 
            {
                User = f.SenderId == userId ? f.Receiver : f.Sender, 
                FriendSince = f.UpdatedAt ?? f.CreatedAt
            })
            .Select(x => new FriendshipResponse(
                x.User.Id,
                x.User.Username,
                x.User.ProfilePictureUrl,
                x.FriendSince,
                FriendshipStatus.Accepted
            ))
            .ToListAsync();

        return Result.Success((IEnumerable<FriendshipResponse>)friends);
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetReceivedFriendRequestsAsync(Guid userId)
    {
        var requests = await _context.Friendships
            .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Sender)
            .AsNoTracking()
            .Select(x => new FriendshipResponse(
                x.Sender.Id,
                x.Sender.Username,
                x.Sender.ProfilePictureUrl,
                x.CreatedAt,
                x.Status
            ))
            .ToListAsync();

        return Result.Success((IEnumerable<FriendshipResponse>)requests);
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetSentFriendRequestsAsync(Guid userId)
    {
        var requests = await _context.Friendships
            .Where(f => f.SenderId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Receiver)
            .AsNoTracking()
            .Select(x => new FriendshipResponse(
                x.Sender.Id,
                x.Sender.Username,
                x.Sender.ProfilePictureUrl,
                x.CreatedAt,
                x.Status
            ))
            .ToListAsync();

        return Result.Success((IEnumerable<FriendshipResponse>)requests);
    }

    public async Task<Result> RemoveFriendAsync(Guid currentUserId, Guid friendId)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.SenderId == currentUserId && f.ReceiverId == friendId) ||
                 (f.SenderId == friendId && f.ReceiverId == currentUserId)));

        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> SendFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId)
            return Result.Failure(new Error("AgurmentException", "senderId is recieverId must not the same id"));

        var existingFriendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.SenderId == senderId && f.ReceiverId == receiverId) ||
                (f.SenderId == receiverId && f.ReceiverId == senderId));

        if (existingFriendship is not null)
            return Result.Failure(new Error("FriendshipExist", "Friendship already exist"));

        var friendship = new Friendship()
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();

        // Send notification:
        await _notificationService.CreateNotificationAsync(
            recipientId: receiverId,
            triggerId: senderId,
            type: NotificationType.NewFriendRequest,
            targetId: senderId
        );

        return Result.Success();
    }
}
