using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Chat;

public record class ConversationResponse(
    Guid Id,
    string? Name,
    ConversationType Type,
    DateTime CreatedAt,
    Guid? LastMessageId,
    DateTime? LastMessageTimestamp,
    List<ParticipantResponse> Participants
);
