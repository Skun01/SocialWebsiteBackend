using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class Notification
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid RecipientUserId { get; set; }
    [ForeignKey("RecipientUserId")]
    public virtual User Recipient { get; set; } = null!;

    [Required]
    public Guid TriggeredByUserId { get; set; }
    [ForeignKey("TriggeredByUserId")]
    public virtual User TriggeredByUser { get; set; } = null!;

    // Loại thông báo
    public NotificationType Type { get; set; }

    // path to navigation to post or comment:
    public string Link { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
