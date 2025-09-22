using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data;

public class SocialWebsiteContext : DbContext
{
    public DbSet<User> Users { set; get; }
    public DbSet<Like> Likes { set; get; }
    public DbSet<Comment> Comments { set; get; }
    public DbSet<Post> Posts { set; get; }
    public DbSet<Friendship> Friendships { set; get; }
    public DbSet<PasswordResetToken> PasswordResetTokens { set; get; }
    public DbSet<FileAsset> FileAssets { set; get; }
    public DbSet<PostFile> PostFiles { set; get; }
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
            likeEntity.HasIndex(l => new { l.UserId, l.TargetId, l.Type }).IsUnique();
            likeEntity.HasIndex(l => l.UserId);
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

        modelBuilder.Entity<PostFile>(entity =>
        {
            entity
                .HasOne(pf => pf.FileAsset)
                .WithOne(fa => fa.PostFile)
                .HasForeignKey<PostFile>(pf => pf.FileAssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(cp => new { cp.ConversationId, cp.UserId });
            entity
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId);
            entity
                .HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
}
