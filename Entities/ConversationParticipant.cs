using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class ConversationParticipant
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; } 

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public Guid? LastReadMessageId { get; set; }

    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
