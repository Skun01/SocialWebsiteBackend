using SocialWebsite.DTOs.Post;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IPostService
{
    Task<Result> DeletePostAsync(Guid postId);
    Task<Result<PostResponse>> CreatePostAsync(CreatePostRequest request);
    Task<Result<PostResponse>> EditPostAsync(EditPostRequest request);
    Task<Result<PostResponse>> GetPostByIdAsync(Guid postId);
    Task<Result<IEnumerable<PostResponse>>> GetAllPostAsync();
}
