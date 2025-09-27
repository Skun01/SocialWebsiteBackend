using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ICacheService _cacheService;
    private readonly SocialWebsiteContext _context;
    public PostRepository(SocialWebsiteContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
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

        // remove cache
        _cacheService.Remove($"Post_{entity.Id}");
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

    public async Task<CursorList<PostResponse>> GetPostsResponseAsync(PostQueryParameters query, string baseUrl)
    {
        var baseQuery = _context.Posts.AsNoTracking();

        baseQuery = baseQuery
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);

        var decodedCursor = CursorHelper.DecodeCursor(query.Cursor);
        if (decodedCursor.HasValue)
        {
            var (cursorCreatedAt, cursorId) = decodedCursor.Value;
            baseQuery = baseQuery.Where(p =>
                p.CreatedAt < cursorCreatedAt ||
                (p.CreatedAt == cursorCreatedAt && p.Id.CompareTo(cursorId) < 0)
            );
        }

        var posts = await baseQuery
        .Take(query.PageSize + 1)
        .Select(p => new PostResponse(
            p.Id,
            p.User.Id,
            p.User.Username,
            p.User.ProfilePictureUrl ?? "",
            p.Content,
            p.Privacy,
            _context.Likes.Count(l => l.Type == LikeType.Post && l.TargetId == p.Id),
            _context.Likes.Any(l => l.Type == LikeType.Post && l.TargetId == p.Id),
            p.Comments.Count,
            p.Files.Select(f => f.ToResponse(baseUrl)).ToList(),
            p.CreatedAt,
            p.UpdatedAt
        ))
        .ToListAsync();

        bool hasNextPage = posts.Count > query.PageSize;
        string? nextCursor = null;
        if (hasNextPage)
        {
            posts.RemoveAt(query.PageSize);
            var lastItem = posts.Last();
            if(lastItem != null)
                nextCursor = CursorHelper.EncodeCursor(lastItem.CreatedAt, lastItem.Id);
        }

        return new CursorList<PostResponse>(posts, nextCursor, hasNextPage);
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        // check if data exist in cache:
        string cacheKey = $"Post_{id}";

        if (_cacheService.TryGetValue<Post>(cacheKey, out var cachedPost) && cachedPost != null)
            return cachedPost;

        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Files)
                .ThenInclude(pf => pf.FileAsset)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        
        // save it in cache if not
        if (post != null)
            _cacheService.Set(cacheKey, post, TimeSpan.FromMinutes(10));
        
        return post;
    }

    public async Task UpdateAsync(Post entity)
    {
        _context.Posts.Update(entity);
        await _context.SaveChangesAsync();

        // remove cache
        _cacheService.Remove($"Post_{entity.Id}");
    }

    public async Task UpdatePrivacy(Guid postId, PostPrivacy privacy)
    {
        Post? post = await _context.Posts.FindAsync(postId);
        post!.Privacy = privacy;
        await _context.SaveChangesAsync();
    }
}
