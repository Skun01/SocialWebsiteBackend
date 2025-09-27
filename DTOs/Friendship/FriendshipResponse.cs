using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Friendship;

public record class FriendshipResponse(
    Guid Id, 
    string Username,
    string? ProfilePictureUrl,
    DateTime FriendSince,
    FriendshipStatus Status
);
