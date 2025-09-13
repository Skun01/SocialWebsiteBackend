using SocialWebsite.Shared.Enums;
namespace SocialWebsite.DTOs.Post;

public record class UpdatePostRequest(
    string Content,
    PostPrivacy Privacy
);
