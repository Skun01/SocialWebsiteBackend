using System;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    Task UpdatePrivacy(Guid postId, PostPrivacy privacy);
}
