using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class PasswordResetToken
{
    [Key]
    public Guid Id { set; get; }
    public Guid UserId { set; get; }
    
    [Required]
    [MaxLength(200)]
    public string TokenHash { set; get; } = string.Empty;
    public DateTime ExpirationUtc { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { set; get; }
    public bool IsUsed { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { set; get; } = null!;
}
