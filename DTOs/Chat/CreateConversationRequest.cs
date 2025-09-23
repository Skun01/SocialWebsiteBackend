using SocialWebsite.Shared.Enums;
namespace SocialWebsite.DTOs.Chat;

public record class CreateConversationRequest(
    Guid RecipientUserId,
    string? Name = null,
    ConversationType Type = ConversationType.OneToOne
);
