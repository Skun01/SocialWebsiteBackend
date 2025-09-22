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
    public Guid ConversationId { get; set; }
    public Guid? ParentMessageId { get; set; }

    [ForeignKey("ConversationId")]
    public virtual Conversation? Conversation { get; set; }

    [ForeignKey("SenderId")]
    public virtual User? Sender { get; set; }

    [ForeignKey("ParentMessageId")]
    public virtual Message? ParentMessage { get; set; }
    public virtual List<MessageReadStatus> ReadByUsers { get; set; } = [];
}
