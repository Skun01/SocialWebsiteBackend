namespace SocialWebsite.DTOs.Chat;

public record class CreateConversationRequest(
    Guid RecipientUserId
);
