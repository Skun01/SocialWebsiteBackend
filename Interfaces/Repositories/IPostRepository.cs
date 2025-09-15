using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    Task UpdatePrivacy(Guid postId, PostPrivacy privacy);
    Task<IEnumerable<PostResponse>> GetAllResponseAsync(string baseUrl, Guid currentUserId);

}
