using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Hubs;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class ChatService : IChatService
{
    private readonly SocialWebsiteContext _context;
    private readonly IUserRepository _userRepo;
    private readonly IHubContext<ChatHub> _hubContext;
    public ChatService(SocialWebsiteContext context, IUserRepository userRepository, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _userRepo = userRepository;
        _hubContext = hubContext;
    }
    public async Task<Result<ConversationResponse>> CreateConversationAsync(Guid creatorUserId, CreateConversationRequest request)
    {
        var existConversation = await _context.ConversationParticipants
            .Where(cp => cp.UserId == creatorUserId)
            .Select(cp => cp.Conversation)
            .Where(c => c!.Participants.Any(p2 => p2.UserId == request.RecipientUserId))
            .FirstOrDefaultAsync();
        if (existConversation != null)
            Result.Failure<ConversationResponse>(new Error("Conversation.Exist", "Conversation already exist"));

        var recipientUser = await _userRepo.GetByIdAsync(request.RecipientUserId);
        if(recipientUser is null)
            Result.Failure<ConversationResponse>(new Error("User.NotFound", "Receipient not found"));

        var newConversation = new Conversation()
        {
            Type = request.Type,
            Name = request.Name ?? recipientUser!.Username
        };

        var creatorParticipant = new ConversationParticipant
        {
            UserId = creatorUserId,
            Conversation = newConversation
        };

        var recipientParticipant = new ConversationParticipant
        {
            UserId = request.RecipientUserId,
            Conversation = newConversation
        };

        _context.Conversations.Add(newConversation);
        _context.ConversationParticipants.AddRange(creatorParticipant, recipientParticipant);

        await _context.SaveChangesAsync(); 

        return Result.Success(newConversation.ToConversationResponse());
    }

    public async Task<Result<MessageResponse>> CreateMessageAsync(Guid conversationId, Guid senderId, CreateMessageRequest request)
    {
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == senderId);
        if (!isParticipant)
            return Result.Failure<MessageResponse>(new Error("User.NotAllow", "You do not belong to this conversation"));

        if (request.ParentMessageId is not null)
        {
            var parentMessage = await _context.Messages.FindAsync(request.ParentMessageId);
            if (parentMessage is null)
                return Result.Failure<MessageResponse>(new Error("Message.NotFound", "Message not found"));

        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = request.Content,
            ParentMessageId = request.ParentMessageId,
            Timestamp = DateTime.UtcNow
        };

        var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);
        conversation!.LastMessageId = message.Id;

        _context.Messages.Add(message);
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
        var messageResponse = message.ToMessageResponse();

        // send message sender event:
        await _hubContext.Clients
            .Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", messageResponse);

        return Result.Success(messageResponse);
    }

    public async Task<Result<CursorList<MessageResponse>>> GetConversationMessagesAsync(
        Guid conversationId,
        Guid userId,
        MessageQueryParameters query)
    {
        var baseQuery = _context.Messages.AsNoTracking();
        baseQuery = baseQuery
            .Where(m => m.ConversationId == conversationId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.Timestamp)
            .ThenByDescending(m => m.Id);

        var decodedCursor = CursorHelper.DecodeCursor(query.Cursor);
        if (decodedCursor.HasValue)
        {
            var (cursorCreatedAt, cursorId) = decodedCursor.Value;
            baseQuery = baseQuery
                .Where(m => m.Timestamp < cursorCreatedAt || (m.Timestamp == cursorCreatedAt && m.Id < cursorId));
        }

        var messages = await baseQuery
            .Take(query.PageSize + 1)
            .Select(m => m.ToMessageResponse())
            .ToListAsync();

        bool hasNextPage = messages.Count > query.PageSize;
        string? nextCursor = null;
        if (hasNextPage)
        {
            messages.RemoveAt(query.PageSize);
            var lastItem = messages.Last();
            if (lastItem != null)
                nextCursor = CursorHelper.EncodeCursor(lastItem.Timestamp, lastItem.Id);
        }
        return Result.Success(new CursorList<MessageResponse>(messages, nextCursor, hasNextPage));

    }

    public async Task<IEnumerable<Guid>> GetUserConversationIdsAsync(Guid userId)
    {
        return await _context.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Select(cp => cp.ConversationId)
            .ToListAsync();
    }

    public async Task<Result<List<ConversationResponse>>> GetUserConversationsAsync(Guid userId)
    {
        var conversations = await _context.Conversations
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender) 
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .ToListAsync();

        var conversationsResponse = conversations.Select(c =>
        {
            var lastMessage = c.Messages
                            .OrderByDescending(m => m.Timestamp)
                            .FirstOrDefault();
            var lastPersonSendMessage = lastMessage?.Sender;

            return new ConversationResponse(
                c.Id,
                DisplayName: c.Name ?? lastPersonSendMessage?.Username ?? "Cuộc trò chuyện",
                c.Type,
                lastMessage?.ToMessageResponse() 
            );
        }).ToList();

        return Result.Success(conversationsResponse);
    }
}
