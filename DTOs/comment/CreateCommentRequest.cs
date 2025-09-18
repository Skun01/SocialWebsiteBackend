namespace SocialWebsite.DTOs.Comment;

public record class CreateCommentRequest(
    Guid PostId,
    Guid UserId,
    Guid? ParentCommentId,
    string Content
);
