using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Hubs;

public class FeedHub : Hub
{
    private readonly IFriendshipService _friendshipService;

    public FeedHub(IFriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdString = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            // Add user to their personal feed group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Add user to friends' feed groups so they can see friends' posts
            var friends = await _friendshipService.GetFriendsAsync(userId);
            if (friends.IsSuccess)
            {
                foreach (var friend in friends.Value)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{friend.Id}");
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdString = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            // Remove from personal feed group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Remove from friends' feed groups
            var friends = await _friendshipService.GetFriendsAsync(userId);
            if (friends.IsSuccess)
            {
                foreach (var friend in friends.Value)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{friend.Id}");
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Method to join a specific user's feed (for when friendship status changes)
    public async Task JoinUserFeed(string targetUserId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{targetUserId}");
    }

    // Method to leave a specific user's feed (for when friendship is removed)
    public async Task LeaveUserFeed(string targetUserId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{targetUserId}");
    }
}
