namespace SocialWebsite.DTOs.Chat;

public record class MessageResponse(
    Guid Id,
    string Content,
    DateTime Timestamp,
    Guid? ParentMessageId,
    Guid SenderId
);
