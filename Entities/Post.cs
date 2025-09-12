using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class Post
{
    [Key]
    public Guid Id { set; get; }
    public Guid UserId { set; get; }
    [Required]
    [MaxLength(3000)]
    public string Content { set; get; } = string.Empty;
    [Column(TypeName = "nvarchar(max)")]
    public string? ImageUrls { set; get; }
    public PostPrivacy Privacy { set; get; } = PostPrivacy.Public;
    public bool IsDeleted { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { set; get; }
    public virtual User User { set; get; } = null!;
    public virtual List<Comment> Comments { set; get; } = [];
    public virtual List<Like> Likes { set; get; } = [];
    public virtual List<PostFile> Files { set; get; } = [];

}
