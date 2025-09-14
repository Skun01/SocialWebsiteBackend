using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;

public class PostFileRepository : IPostFileRepository
{
    private readonly SocialWebsiteContext _context;

    public PostFileRepository(SocialWebsiteContext context)
    {
        _context = context;
    }

    public async Task<PostFile> AddAsync(PostFile entity)
    {
        _context.PostFiles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<PostFile>> AddRangeAsync(IEnumerable<PostFile> postFiles)
    {
        _context.PostFiles.AddRange(postFiles);
        await _context.SaveChangesAsync();
        return postFiles;
    }

    public async Task DeleteAsync(PostFile entity)
    {
        _context.PostFiles.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PostFile>> GetAllAsync()
    {
        return await _context.PostFiles
            .Include(p => p.FileAsset)
            .ToListAsync();
    }

    public async Task<PostFile?> GetByIdAsync(Guid id)
    {
        return await _context.PostFiles
            .Include(p => p.FileAsset)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(PostFile entity)
    {
        _context.PostFiles.Update(entity);
        await _context.SaveChangesAsync();
    }
}
