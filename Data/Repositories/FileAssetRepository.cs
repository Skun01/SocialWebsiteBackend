using System;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;

namespace SocialWebsite.Data.Repositories;

public class FileAssetRepository : IFileAssetRepository
{
    private readonly SocialWebsiteContext _context;
    public FileAssetRepository(SocialWebsiteContext context)
    {
        _context = context;
    }
    public async Task<FileAsset> AddAsync(FileAsset entity)
    {
        _context.FileAssets.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<FileAsset>> AddRangeAsync(List<FileAsset> fileAssets)
    {
        _context.FileAssets.AddRange(fileAssets);
        await _context.SaveChangesAsync();
        return fileAssets;
    }

    public async Task DeleteAsync(FileAsset entity)
    {
        _context.FileAssets.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<FileAsset>> GetAllAsync()
    {
        return await _context.FileAssets.ToListAsync();
    }

    public async Task<FileAsset?> GetByIdAsync(Guid id)
    {
        return await _context.FileAssets.FirstOrDefaultAsync(fa => fa.Id == id);
    }

    public async Task UpdateAsync(FileAsset entity)
    {
        _context.FileAssets.Update(entity);
        await _context.SaveChangesAsync();
    }
}
