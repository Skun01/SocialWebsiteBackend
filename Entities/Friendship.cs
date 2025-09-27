using System;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Entities;

public class Friendship
{
    public Guid Id { set; get; }
    public Guid SenderId { set; get; }
    public Guid ReceiverId { set; get; }
    public FriendshipStatus Status { set; get; } = FriendshipStatus.Pending;
    public virtual User Sender { set; get; } = null!;
    public virtual User Receiver { set; get; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
