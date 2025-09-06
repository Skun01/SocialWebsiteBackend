using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;

namespace SocialWebsite.Data;

public class SocialWebsiteContext : DbContext
{
    public DbSet<User> Users { set; get; }
    public DbSet<Like> Likes { set; get; }
    public DbSet<Comment> Comments { set; get; }
    public DbSet<Post> Posts { set; get; }
    public DbSet<Friendship> Friendships { set; get; }
    public SocialWebsiteContext(DbContextOptions<SocialWebsiteContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(userEntity =>
        {
            userEntity
                .Property(u => u.Gender)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            userEntity
                .Property(u => u.IsEmailVerified)
                .HasDefaultValue(false);
            userEntity
            .Property(u => u.IsActive)
            .HasDefaultValue(false);

            userEntity
                .HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Post>(postEntity =>
        {
            postEntity
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            postEntity.HasIndex(p => p.UserId);
        });

        modelBuilder.Entity<Comment>(commentEnitity =>
        {
            commentEnitity
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            commentEnitity
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
            commentEnitity
                .HasOne(c => c.ParentComment)
                .WithMany(pc => pc.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);
            commentEnitity.HasIndex(c => c.UserId);
            commentEnitity.HasIndex(c => c.PostId);
        });

        modelBuilder.Entity<Like>(likeEntity =>
        {
            likeEntity
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            likeEntity.HasIndex(l => new { l.UserId, l.TargetId }).IsUnique();
            likeEntity.HasIndex(l => l.TargetId);
        });

        modelBuilder.Entity<Friendship>(friendshipEntity =>
        {
            friendshipEntity
                .HasOne(f => f.Sender)
                .WithMany(u => u.SentFriendships)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            friendshipEntity
                .HasOne(f => f.Receiver)
                .WithMany(u => u.ReceivedFriendships)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            friendshipEntity
                .HasIndex(f => new { f.SenderId, f.ReceiverId })
                .IsUnique();
        });

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
}
