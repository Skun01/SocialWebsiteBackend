using System;
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
    public Task<FileAsset> AddAsync(FileAsset entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<FileAsset>> AddRangeAsync(List<FileAsset> fileAssets)
    {
        _context.FileAssets.AddRange(fileAssets);
        await _context.SaveChangesAsync();
        return fileAssets;
    }

    public Task DeleteAsync(FileAsset entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FileAsset>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<FileAsset?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(FileAsset entity)
    {
        throw new NotImplementedException();
    }
}
