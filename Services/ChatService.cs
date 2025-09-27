using System;
using Microsoft.AspNetCore.SignalR;
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
    private readonly IChatRepository _chatRepo;
    private readonly IUserRepository _userRepo;
    private readonly IHubContext<ChatHub> _hubContext;
    
    public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IHubContext<ChatHub> hubContext)
    {
        _chatRepo = chatRepository;
        _userRepo = userRepository;
        _hubContext = hubContext;
    }
    public async Task<Result<ConversationResponse>> CreateConversationAsync(Guid creatorUserId, CreateConversationRequest request)
    {
        var existConversation = await _chatRepo.GetConversationByParticipantsAsync(creatorUserId, request.RecipientUserId);
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

        await _chatRepo.AddConversationAsync(newConversation);
        
        var creatorParticipant = new ConversationParticipant
        {
            UserId = creatorUserId,
            ConversationId = newConversation.Id
        };

        var recipientParticipant = new ConversationParticipant
        {
            UserId = request.RecipientUserId,
            ConversationId = newConversation.Id
        };

        await _chatRepo.AddParticipantAsync(creatorParticipant);
        await _chatRepo.AddParticipantAsync(recipientParticipant);

        return Result.Success(newConversation.ToConversationResponse());
    }

    public async Task<Result<MessageResponse>> CreateMessageAsync(Guid conversationId, Guid senderId, CreateMessageRequest request)
    {
        var isParticipant = await _chatRepo.IsUserInConversationAsync(conversationId, senderId);
        if (!isParticipant)
            return Result.Failure<MessageResponse>(new Error("User.NotAllow", "You do not belong to this conversation"));

        if (request.ParentMessageId is not null)
        {
            var parentMessage = await _chatRepo.GetMessageByIdAsync(request.ParentMessageId.Value);
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

        await _chatRepo.AddMessageAsync(message);
        
        // Update conversation's last message
        var conversation = await _chatRepo.GetConversationByParticipantsAsync(senderId, conversationId);
        if (conversation != null)
        {
            conversation.LastMessageId = message.Id;
            await _chatRepo.UpdateConversationAsync(conversation);
        }
        
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
        // Verify user is in conversation
        var isParticipant = await _chatRepo.IsUserInConversationAsync(conversationId, userId);
        if (!isParticipant)
            return Result.Failure<CursorList<MessageResponse>>(new Error("User.NotAllow", "You do not belong to this conversation"));
            
        var messages = await _chatRepo.GetConversationMessagesAsync(conversationId, query);
        return Result.Success(messages);

    }

    public async Task<IEnumerable<Guid>> GetUserConversationIdsAsync(Guid userId)
    {
        return await _chatRepo.GetUserConversationIdsAsync(userId);
    }

    public async Task<Result<List<ConversationResponse>>> GetUserConversationsAsync(Guid userId)
    {
        var conversationsResponse = await _chatRepo.GetUserConversationsAsync(userId);
        return Result.Success(conversationsResponse);
    }
}
