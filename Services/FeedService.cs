using Microsoft.AspNetCore.SignalR;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Hubs;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class FeedService : IFeedService
{
    private readonly IHubContext<FeedHub> _feedHubContext;

    public FeedService(IHubContext<FeedHub> feedHubContext)
    {
        _feedHubContext = feedHubContext;
    }

    public async Task NotifyNewPostAsync(Guid authorId, PostResponse post)
    {
        // Send to author's feed group (includes author and their friends)
        await _feedHubContext.Clients.Group($"user_{authorId}")
            .SendAsync("NewPost", post);
    }

    public async Task NotifyPostUpdatedAsync(Guid authorId, PostResponse post)
    {
        // Send to author's feed group
        await _feedHubContext.Clients.Group($"user_{authorId}")
            .SendAsync("PostUpdated", post);
    }

    public async Task NotifyPostDeletedAsync(Guid authorId, Guid postId)
    {
        // Send to author's feed group
        await _feedHubContext.Clients.Group($"user_{authorId}")
            .SendAsync("PostDeleted", new { postId = postId, authorId = authorId });
    }
}
