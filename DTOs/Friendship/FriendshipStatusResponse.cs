using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Friendship;

public record class FriendshipStatusResponse(
    FriendshipStatus Status,
    string Message
);
