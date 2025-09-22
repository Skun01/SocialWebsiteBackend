using System;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IChatService
{
    Task<ConversationResponse> CreateConversationAsync(Guid creatorUserId, Guid recipientUserId);
    Task<MessageResponse> CreateMessageAsync(Guid conversationId, Guid senderId, string content);
    Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId);
    Task<CursorList<MessageResponse>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int pageSize, string? nextCursor);
}
