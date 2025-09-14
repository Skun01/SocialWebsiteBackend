using System;
using System.ComponentModel.DataAnnotations;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class FileAsset
{
    [Key]
    public Guid Id { set; get; }
    public string StorageKey { set; get; } = string.Empty;

    [Required, MaxLength(128)]
    public string MimeType { set; get; } = string.Empty;
    public long FileSizeBytes { set; get; }
    public string? OriginalFileName { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
    
    public virtual PostFile? PostFile { set; get; }
}
