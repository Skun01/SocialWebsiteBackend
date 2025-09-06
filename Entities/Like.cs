using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWebsite.Entities;

public class Like
{
    [Key]
    public Guid Id { set; get; }
    public Guid UserId { set; get; }
    public Guid TargetId { set; get; }
    public string TargetType { set; get; } = string.Empty; // Post, Comment
    public virtual User User { set; get; } = null!;
}
