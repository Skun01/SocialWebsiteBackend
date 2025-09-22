using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class MessageReadStatus
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    public virtual Message? Message { get; set; }
    public virtual User? User { get; set; }
}
