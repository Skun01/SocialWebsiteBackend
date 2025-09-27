using System;
using SocialWebsite.DTOs.Friendship;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IFriendshipService
{
    Task<Result> SendFriendRequestAsync(Guid senderId, Guid receiverId);
    Task<Result> AcceptFriendRequestAsync(Guid senderId, Guid receiverId);
    Task<Result> DeclineFriendRequestAsync(Guid senderId, Guid receiverId);
    Task<Result> RemoveFriendAsync(Guid currentUserId, Guid friendId);
    Task<Result<IEnumerable<FriendshipResponse>>> GetFriendsAsync(Guid userId);
    Task<Result<IEnumerable<FriendshipResponse>>> GetReceivedFriendRequestsAsync(Guid userId);
    Task<Result<IEnumerable<FriendshipResponse>>> GetSentFriendRequestsAsync(Guid userId);
}
