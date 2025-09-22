using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [Required]
    public Guid SenderId { get; set; }
    [ForeignKey("SenderId")]
    public User? Sender { get; set; }
    public Guid ConversationId { get; set; }
    [ForeignKey("ConversationId")]
    public Conversation? Conversation { get; set; }
}
