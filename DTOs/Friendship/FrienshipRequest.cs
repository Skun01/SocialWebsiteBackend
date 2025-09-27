namespace SocialWebsite.DTOs.Friendship;

public record class FrienshipRequest(
    Guid RequestId,
    DateTime SentAts
);