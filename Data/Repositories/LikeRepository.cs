using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly SocialWebsiteContext _context;
    public LikeRepository(SocialWebsiteContext context)
    {
        _context = context;
    }
    public async Task<Like> CreateLikeAsync(Guid userId, Guid TargetId, LikeType type)
    {
        Like newLike = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TargetId = TargetId,
            Type = type
        };
        _context.Likes.Add(newLike);
        await _context.SaveChangesAsync();
        return newLike;
    }

    public async Task DeleteLikeAsync(Guid likeId)
    {
        Like? target = await _context.Likes.FindAsync(likeId);
        _context.Likes.Remove(target!);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Like>> GetLikesByTargetId(Guid targetId, LikeType type)
    {
        return await _context.Likes
            .Where(l => l.TargetId == targetId && l.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<Like>> GetLikesByUserId(Guid userId)
    {
        return await _context.Likes
            .Where(l => l.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> GetTargetLikeNumber(Guid targetId, LikeType type)
    {
        return await _context.Likes
            .Where(l => l.TargetId == targetId && l.Type == type)
            .CountAsync();
    }

    public async Task<bool> IsUserLiked(Guid? userId, Guid targetId, LikeType type)
    {
        if (userId is null)
            return false;
        return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.TargetId == targetId && l.Type == type);
    }
}
