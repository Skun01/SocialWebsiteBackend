using SocialWebsite.DTOs.User;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class PostResponse(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string AuthorAvatar,
    string Content,
    PostPrivacy Privacy,
    int LikeCount,
    bool IsLikedByMe,
    int CommentCount,
    List<PostFileResponse> Files,
    DateTime CreatedAt,
    DateTime UpdatedAt  
);
