using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface IFileAssetRepository : IGenericRepository<FileAsset>
{
    public Task<IEnumerable<FileAsset>> AddRangeAsync(List<FileAsset> fileAssets);
}
