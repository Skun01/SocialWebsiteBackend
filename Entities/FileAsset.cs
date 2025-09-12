using System;
using System.ComponentModel.DataAnnotations;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class FileAsset
{
    [Key]
    public Guid Id { set; get; }

    [Required, MaxLength(128)]
    public string MimeType { set; get; } = string.Empty;
    public long FileSizeBytes { set; get; }
    public string? OriginalFileName { set; get; }
    public AssetType Type { set; get; } = AssetType.Other;
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
}
