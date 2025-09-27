using System;
using SocialWebsite.DTOs.Friendship;
using SocialWebsite.Entities;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface IFriendshipRepository
{
    Task<Friendship?> GetFriendshipAsync(Guid senderId, Guid receiverId, FriendshipStatus status);
    Task<IEnumerable<FriendshipResponse>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<FriendshipResponse>> GetReceivedFriendRequestsAsync(Guid userId);
    Task<IEnumerable<FriendshipResponse>> GetSentFriendRequestsAsync(Guid userId);
    Task<bool> AreFriendsAsync(Guid userId1, Guid userId2);
    Task<FriendshipStatus?> GetFriendshipStatusAsync(Guid userId1, Guid userId2);
    Task<Friendship> UpdateAsync(Friendship entity);
    Task<IEnumerable<Friendship>> GetAllAsync();
    Task<bool> DeleteAsync(Guid id);
    Task<Friendship> AddAsync(Friendship entity);
}