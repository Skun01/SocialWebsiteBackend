using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    Task UpdatePrivacy(Guid postId, PostPrivacy privacy);
    Task<CursorList<PostResponse>> GetPostsResponseAsync(PostQueryParameters query, string baseUrl);
    Task<CursorList<PostResponse>> GetPostsByUserIdResponseAsync(Guid userId, PostQueryParameters query, string baseUrl, Guid? currentUserId = null);

}
