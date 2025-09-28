using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SocialWebsiteContext _context;
    public UserRepository(SocialWebsiteContext context)
    {
        _context = context;
    }
    public async Task<User> AddAsync(User entity)
    {
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUserEmailExistAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUserNameExistAsync(string userName)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == userName);
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVerifyEmailByIdAsync(Guid userId, bool IsEmailVerified)
    {
        User? user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
            return;

        user.IsEmailVerified = IsEmailVerified;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserExistAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<PageList<UserResponse>> SearchAsync(UserQueryParameters query, Guid? currentUserId)
    {
        var baseQuery = _context.Users
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            baseQuery = baseQuery.Where(u =>
                u.Username.Contains(query.Name)
            );
        }

         baseQuery = (query.SortBy?.ToLowerInvariant()) switch
        {
            "name" => baseQuery.OrderBy(u => u.Username).ThenBy(u => u.Id),
            "recent" => baseQuery.OrderByDescending(u => u.Id),
            _ => baseQuery.OrderBy(u => u.Username).ThenBy(u => u.Id)
        };

        var total = await baseQuery.CountAsync();

         var items = await baseQuery
            .Skip((Math.Max(1, query.PageNumber) - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => u.ToResponse())   
            .ToListAsync();

        return new PageList<UserResponse>(items, total, Math.Max(1, query.PageNumber), query.PageSize);
    }

    public async Task UpdateUserRoleAsync(Guid userId, UserRole role)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return;
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
