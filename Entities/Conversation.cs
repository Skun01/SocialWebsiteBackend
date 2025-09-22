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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ConversationParticipant> Participants { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
}
