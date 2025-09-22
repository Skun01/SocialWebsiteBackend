using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class ChatService : IChatService
{
    private readonly SocialWebsiteContext _context;
    public ChatService(SocialWebsiteContext context)
    {
        _context = context;
    }

    public Task<ConversationResponse> CreateConversationAsync(Guid creatorUserId, Guid recipientUserId)
    {
        throw new NotImplementedException();
    }

    public Task<MessageResponse> CreateMessageAsync(Guid conversationId, Guid senderId, string content)
    {
        throw new NotImplementedException();
    }

    public Task<CursorList<MessageResponse>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int pageSize, string? nextCursor)
    {
        throw new NotImplementedException();
    }

    public Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
