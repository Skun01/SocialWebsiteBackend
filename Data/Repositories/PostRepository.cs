using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Mapping;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class PostRepository : IPostRepository
{
    private readonly SocialWebsiteContext _context;
    public PostRepository(SocialWebsiteContext context)
    {
        _context = context;
    }
    public async Task<Post> AddAsync(Post entity)
    {
        _context.Posts.Add(entity);
        await _context.SaveChangesAsync();
        Post? newPost = await _context.Posts
        .Include(p => p.User)
        .FirstOrDefaultAsync(p => p.Id == entity.Id);
        return newPost!;
    }

    public async Task DeleteAsync(Post entity)
    {
        _context.Posts.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Files)
                .ThenInclude(pf => pf.FileAsset)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<PostResponse>> GetAllResponseAsync(string baseUrl, Guid currentUserId)
    {
        return await _context.Posts
        .AsNoTracking()
        .OrderByDescending(p => p.CreatedAt)
        .Select(p => new PostResponse(
            p.Id,
            p.User.Id,
            p.User.Username,
            p.User.ProfilePictureUrl ?? "",
            p.Content,
            p.Privacy,
            _context.Likes.Count(l => l.Type == LikeType.Post && l.TargetId == p.Id),
            _context.Likes.Any(l => l.Type == LikeType.Post && l.TargetId == p.Id && l.UserId == currentUserId),
            p.Comments.Count,
            p.Files.Select(f => f.ToResponse(baseUrl)).ToList(),
            p.CreatedAt,
            p.UpdatedAt
        ))
        .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Files)
                .ThenInclude(pf => pf.FileAsset)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Post entity)
    {
        _context.Posts.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePrivacy(Guid postId, PostPrivacy privacy)
    {
        Post? post = await _context.Posts.FindAsync(postId);
        post!.Privacy = privacy;
        await _context.SaveChangesAsync();
    }
}
