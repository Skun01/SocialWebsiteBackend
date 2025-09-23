using System;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IChatService
{
    Task<Result<ConversationResponse>> CreateConversationAsync(Guid creatorUserId, CreateConversationRequest request);
    Task<Result<MessageResponse>> CreateMessageAsync(Guid conversationId, Guid senderId, CreateMessageRequest request);
    Task<Result<List<ConversationResponse>>> GetUserConversationsAsync(Guid userId);
    Task<Result<CursorList<MessageResponse>>> GetConversationMessagesAsync(Guid conversationId, Guid userId, MessageQueryParameters query);
    Task<IEnumerable<Guid>> GetUserConversationIdsAsync(Guid userId);
}
