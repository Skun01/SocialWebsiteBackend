using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class CreatePostRequest(
    Guid UserId,
    string Content,
    PostPrivacy Privacy
);