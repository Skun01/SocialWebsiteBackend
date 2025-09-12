using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class PostService : IPostService
{
    public Task<Result<PostResponse>> CreatePostAsync(CreatePostRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeletePostAsync(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PostResponse>> EditPostAsync(EditPostRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<PostResponse>>> GetAllPostAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<PostResponse>> GetPostByIdAsync(Guid postId)
    {
        throw new NotImplementedException();
    }
}
