using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWebsite.Entities;

public class Comment
{
    [Key]
    public Guid Id { set; get; }
    public Guid PostId { set; get; }
    public Guid UserId { set; get; }
    public Guid? ParentCommentId { set; get; }
    public string Content { set; get; } = string.Empty;
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { set; get; } = DateTime.UtcNow;
    public bool IsDeleted { set; get; } = false;

    public virtual Post Post { set; get; } = null!;
    public virtual User User { set; get; } = null!;
    public virtual Comment? ParentComment { set; get; }
    public virtual List<Comment> Replies { set; get; } = [];
    public virtual List<Like> Likes { set; get; } = [];
}
