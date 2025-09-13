using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class PostDetailResponse(
    Guid UserId,
    string Content,
    PostPrivacy Privacy,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
