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
    Task<Result> UpdatePostPrivacyAsync(Guid postId, ChangePostPrivacyRequest request);
    Task<Result<PostResponse>> UpdatePostAsync(Guid postId, UpdatePostRequest request);
    // Task<Result> LikePostAsync(Guid postId, Guid userId);
    // Task<Result> UnlikePostAsync(Guid postId, Guid userId);
}
