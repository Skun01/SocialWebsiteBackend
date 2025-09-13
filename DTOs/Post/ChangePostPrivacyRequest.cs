using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class ChangePostPrivacyRequest(
    Guid PostId,
    PostPrivacy Privacy
);