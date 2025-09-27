using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Data.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly SocialWebsiteContext _context;

    public ChatRepository(SocialWebsiteContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetConversationByParticipantsAsync(Guid userId1, Guid userId2)
    {
        return await _context.ConversationParticipants
            .Where(cp => cp.UserId == userId1)
            .Select(cp => cp.Conversation)
            .Where(c => c!.Participants.Any(p2 => p2.UserId == userId2))
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation> AddConversationAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task<ConversationParticipant> AddParticipantAsync(ConversationParticipant participant)
    {
        await _context.ConversationParticipants.AddAsync(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages.FindAsync(messageId);
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task UpdateConversationAsync(Conversation conversation)
    {
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
    }

    public async Task<CursorList<MessageResponse>> GetConversationMessagesAsync(
        Guid conversationId,
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
        return new CursorList<MessageResponse>(messages, nextCursor, hasNextPage);
    }

    public async Task<IEnumerable<Guid>> GetUserConversationIdsAsync(Guid userId)
    {
        return await _context.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Select(cp => cp.ConversationId)
            .ToListAsync();
    }

    public async Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId)
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

        return conversationsResponse;
    }
}