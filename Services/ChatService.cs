using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class ChatService : IChatService
{
    private readonly SocialWebsiteContext _context;
    public ChatService(SocialWebsiteContext context)
    {
        _context = context;
    }
    public async Task<ConversationResponse> CreateConversationAsync(Guid creatorUserId, Guid recipientUserId)
    {
        var existingConversation = await _context.ConversationParticipants
            .Where(cp => cp.UserId == creatorUserId)
            .Select(cp => cp.Conversation)
            .Where(c => c!.Participants.Any(cp2 => cp2.UserId == recipientUserId))
            .FirstOrDefaultAsync();
        if (existingConversation != null)
            return new ConversationResponse(
                Id: existingConversation.Id,
                Name: existingConversation.Name,
                Type: existingConversation.Type,
                CreatedAt: existingConversation.CreatedAt,
                LastMessageId: existingConversation.LastMessageId,
                LastMessageTimestamp: existingConversation.LastMessageTimestamp,
                Participants: existingConversation.Participants.Select(p => new ParticipantResponse(
                    p.UserId,
                    p.Role,
                    p.JoinedAt
                )).ToList()
            );

        var newConversation = new Conversation();
        var creatorParticipant = new ConversationParticipant
        {
            UserId = creatorUserId,
            Conversation = newConversation
        };

        var recipientParticipant = new ConversationParticipant
        {
            UserId = recipientUserId,
            Conversation = newConversation
        };
        _context.conversations.Add(newConversation);
        _context.ConversationParticipants.AddRange(creatorParticipant, recipientParticipant);
        await _context.SaveChangesAsync();

        return new ConversationResponse(
            Id: newConversation.Id,
            Name: newConversation.Name,
            Type: newConversation.Type,
            CreatedAt: newConversation.CreatedAt,
            LastMessageId: newConversation.LastMessageId,
            LastMessageTimestamp: newConversation.LastMessageTimestamp,
            Participants: newConversation.Participants.Select(p => new ParticipantResponse(
                p.UserId,
                p.Role,
                p.JoinedAt
            )).ToList()
        );;
    }

    public async Task<MessageResponse> CreateMessageAsync(Guid conversationId, Guid senderId, string content)
    {
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == senderId);

        if (!isParticipant)
            throw new UnauthorizedAccessException("User is not in this conversation");

        var newMessage = new Message()
        {
            Content = content,
            SenderId = senderId,
            ConversationId = conversationId,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(newMessage);
        await _context.SaveChangesAsync();
        return new MessageResponse(
            Id: newMessage.Id,
            Content: newMessage.Content,
            Timestamp: newMessage.Timestamp,
            SenderId: newMessage.SenderId,
            ConversationId: newMessage.ConversationId,
            ParentMessageId: newMessage.ParentMessageId
        );
    }
}
