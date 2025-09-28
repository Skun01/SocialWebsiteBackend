using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SocialWebsite.Data;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly SocialWebsiteContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DatabaseSeeder(SocialWebsiteContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Seed Admin User if not exists
            await SeedAdminUserAsync();

            Log.Information("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedAdminUserAsync()
    {
        // Check if any admin user already exists
        var adminExists = await _context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (adminExists)
        {
            Log.Information("Admin user already exists, skipping admin user creation");
            return;
        }

        // Create default admin user
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "thaitruong@gmail.com",
            PasswordHash = "", // Will be set below
            FirstName = "System",
            LastName = "Administrator",
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Hash the default password
        adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "123456789tT");

        // Add to database
        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        Log.Information("Default admin user created successfully with username: {Username}", adminUser.Username);
        Log.Warning("Default admin password is 'Admin@123' - Please change it after first login!");
    }
}
