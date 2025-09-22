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

        // Conversation
        modelBuilder.Entity<Conversation>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(255);
            b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            b.HasIndex(x => x.CreatedAt);
            b.Property(x => x.Type)
             .HasConversion<int>()
             .HasDefaultValue(ConversationType.Direct);
        });

        // ConversationParticipan
        modelBuilder.Entity<ConversationParticipant>(b =>
            {
            b.HasKey(x => new { x.ConversationId, x.UserId });
            b.Property(x => x.JoinedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            b.HasOne(x => x.Conversation)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.LastReadMessageId);
        });
        
        // Message
        modelBuilder.Entity<Message>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();
            b.Property(x => x.Content)
            .IsRequired();
            b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            b.Property(x => x.ClientMessageId)
            .HasMaxLength(36);
            b.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Sender)
            .WithMany() 
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.ConversationId, x.CreatedAt });
            b.HasIndex(x => x.SenderId);
            b.HasIndex(x => new { x.ConversationId, x.SenderId, x.ClientMessageId })
            .IsUnique()
            .HasFilter("[ClientMessageId] IS NOT NULL");
        });

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
}
