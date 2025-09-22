using System;
using System.ComponentModel.DataAnnotations;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class Conversation
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(255)]
    public string? Name { get; set; }
    public ConversationType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? LastMessageId { get; set; }
    public DateTime? LastMessageTimestamp { get; set; }

    public virtual List<ConversationParticipant> Participants { get; set; } = [];
    public virtual List<Message> Messages { get; set; } = [];
}
