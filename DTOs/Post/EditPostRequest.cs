namespace SocialWebsite.DTOs.Post;

public record class EditPostRequest(
    Guid PostId,
    string Content
);
