using SocialWebsite.DTOs.Post;

namespace SocialWebsite.Interfaces.Services;

public interface IFeedService
{
    Task NotifyNewPostAsync(Guid authorId, PostResponse post);
    Task NotifyPostUpdatedAsync(Guid authorId, PostResponse post);
    Task NotifyPostDeletedAsync(Guid authorId, Guid postId);
}
