using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;

namespace SocialWebsite.Data;

public class SocialWebsiteContext : DbContext
{
    public DbSet<User> Users { set; get; }
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
        });

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
}
