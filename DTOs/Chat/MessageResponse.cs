namespace SocialWebsite.DTOs.Chat;

public record class MessageResponse(
    Guid Id,
    string Content,
    DateTime Timestamp,
    Guid SenderId,
    Guid ConversationId,
    Guid? ParentMessageId
);
