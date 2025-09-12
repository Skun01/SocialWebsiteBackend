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
    public int SortOrder { set; get; } = 0;
    [ForeignKey(nameof(PostId))]
    public virtual Post Post { set; get; } = null!;
    [ForeignKey(nameof(FileAssetId))]
    public virtual FileAsset FileAsset { set; get; } = null!;
}
