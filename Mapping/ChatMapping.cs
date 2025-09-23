using System;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Mapping;

public static class ChatMapping
{
    public static ParticipantResponse ToParticipantResponse(this ConversationParticipant participant)
    {
        var user = participant.User;
        return new(
            UserId: user!.Id,
            UserName: user.Username,
            Role: participant.Role,
            AvatarUrl: user.ProfilePictureUrl ?? ""
        );
    }
    public static MessageResponse ToMessageResponse(this Message message)
    {
        return new(
            Id: message.Id,
            Content: message.Content,
            Timestamp: message.Timestamp,
            ParentMessageId: message.ParentMessageId ?? null,
            SenderId: message.SenderId
        );
    }

    public static ConversationResponse ToConversationResponse(this Conversation conversation, Message? lastMessage = null)
    {
        return new(
            Id: conversation.Id,
            DisplayName: conversation.Name ?? string.Empty,
            Type: conversation?.Type ?? ConversationType.OneToOne,
            LastMessage: lastMessage?.ToMessageResponse()
        );
    }
}
