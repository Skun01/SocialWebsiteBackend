using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Conversation?> GetConversationByParticipantsAsync(Guid userId1, Guid userId2);
    Task<Conversation> AddConversationAsync(Conversation conversation);
    Task<ConversationParticipant> AddParticipantAsync(ConversationParticipant participant);
    Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId);
    Task<Message?> GetMessageByIdAsync(Guid messageId);
    Task<Message> AddMessageAsync(Message message);
    Task UpdateConversationAsync(Conversation conversation);
    Task<CursorList<MessageResponse>> GetConversationMessagesAsync(Guid conversationId, MessageQueryParameters query);
    Task<IEnumerable<Guid>> GetUserConversationIdsAsync(Guid userId);
    Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId);
}