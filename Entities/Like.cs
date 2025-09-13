using System;
using System.ComponentModel.DataAnnotations;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class Like
{
    [Key]
    public Guid Id { set; get; }
    public Guid UserId { set; get; }
    public Guid TargetId { set; get; }
    public LikeType Type { set; get; } 
    public virtual User User { set; get; } = null!;
}
