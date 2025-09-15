using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class PostFile
{
    [Key]
    public Guid Id { set; get; }
    public Guid PostId { set; get; }
    public Guid FileAssetId { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    [ForeignKey(nameof(FileAssetId))]
    public FileAsset FileAsset { set; get; } = null!;

    [ForeignKey(nameof(PostId))]
    public virtual Post Post { set; get; } = null!;
}
