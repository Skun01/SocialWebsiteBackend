using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Chat;

public record class CreateMessageRequest(
    string Content,
    Guid? ParentMessageId = null
);
