using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    [Required]
    public Guid SenderId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? ClientMessageId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey(nameof(SenderId))]
    public User Sender { get; set; } = null!;
}
