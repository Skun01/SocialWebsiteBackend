namespace SocialWebsite.DTOs.comment;

public record class CommentResponse(
    Guid PostId,
    Guid UserId,
    Guid? ParentCommentId,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int LikeCount,
    int ReplieCount
);