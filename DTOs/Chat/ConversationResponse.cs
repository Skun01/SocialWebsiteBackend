using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Chat;

public record class ConversationResponse(
    Guid Id,
    string DisplayName,
    ConversationType Type,
    MessageResponse? LastMessage
);
