using System;
using SocialWebsite.DTOs.Friendship;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepo;
    private readonly INotificationService _notificationService;
    
    public FriendshipService(IFriendshipRepository friendshipRepository, INotificationService notificationService)
    {
        _friendshipRepo = friendshipRepository;
        _notificationService = notificationService;
    }

    public async Task<Result> AcceptFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        var friendship = await _friendshipRepo.GetFriendshipAsync(senderId, receiverId, FriendshipStatus.Pending);

        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));

        friendship.Status = FriendshipStatus.Accepted;
        friendship.UpdatedAt = DateTime.UtcNow;

        await _friendshipRepo.UpdateAsync(friendship);

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
        var friendship = await _friendshipRepo.GetFriendshipAsync(senderId, receiverId, FriendshipStatus.Pending);

        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));

        await _friendshipRepo.DeleteAsync(friendship.Id);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetFriendsAsync(Guid userId)
    {
        var friends = await _friendshipRepo.GetFriendsAsync(userId);
        return Result.Success(friends);
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetReceivedFriendRequestsAsync(Guid userId)
    {
        var requests = await _friendshipRepo.GetReceivedFriendRequestsAsync(userId);
        return Result.Success(requests);
    }

    public async Task<Result<IEnumerable<FriendshipResponse>>> GetSentFriendRequestsAsync(Guid userId)
    {
        var requests = await _friendshipRepo.GetSentFriendRequestsAsync(userId);
        return Result.Success(requests);
    }

    public async Task<Result> RemoveFriendAsync(Guid currentUserId, Guid friendId)
    {
        var areFriends = await _friendshipRepo.AreFriendsAsync(currentUserId, friendId);
        if (!areFriends)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));
            
        var friendship = await _friendshipRepo.GetFriendshipAsync(currentUserId, friendId, FriendshipStatus.Accepted) 
            ?? await _friendshipRepo.GetFriendshipAsync(friendId, currentUserId, FriendshipStatus.Accepted);
            
        if (friendship == null)
            return Result.Failure(new Error("NotFound", "Friendship not found!"));
            
        await _friendshipRepo.DeleteAsync(friendship.Id);

        return Result.Success();
    }

    public async Task<Result> SendFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId)
            return Result.Failure(new Error("AgurmentException", "senderId is recieverId must not the same id"));

        var status = await _friendshipRepo.GetFriendshipStatusAsync(senderId, receiverId);

        if (status.HasValue)
            return Result.Failure(new Error("FriendshipExist", "Friendship already exist"));

        var friendship = new Friendship()
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            CreatedAt = DateTime.UtcNow
        };

        await _friendshipRepo.AddAsync(friendship);

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
