using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface IPostFileRepository : IGenericRepository<PostFile>
{
    Task<IEnumerable<PostFile>> AddRangeAsync(IEnumerable<PostFile> postFiles);
}
