using System;
using System.ComponentModel.DataAnnotations;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class User
{
    [Key]
    public Guid Id { set; get; }
    [Required, MaxLength(100)]
    public required string Username { get; set; }
    [Required, EmailAddress]
    public required string Email { get; set; }
    [Required, MaxLength(1000)]
    public required string PasswordHash { get; set; }
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual List<Post> Posts { set; get; } = [];
    public virtual List<Comment> Comments { set; get; } = [];
    public virtual List<Like> Likes { set; get; } = [];
    public virtual List<Friendship> SentFriendships { set; get; } = [];
    public virtual List<Friendship> ReceivedFriendships { set; get; } = [];
}
