using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;

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
        return entity;
    }

    public async Task DeleteAsync(Post entity)
    {
        _context.Posts.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Post entity)
    {
        _context.Posts.Update(entity);
        await _context.SaveChangesAsync();
    }
}
