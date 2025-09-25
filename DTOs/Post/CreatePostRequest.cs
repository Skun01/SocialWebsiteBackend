using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class CreatePostRequest(
    string Content,
    PostPrivacy Privacy
);