namespace SocialWebsite.DTOs.Comment;

public record class CreateCommentRequest(
    string Content,
    Guid? ParentCommentId = null
);
