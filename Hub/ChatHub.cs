using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using SocialWebsite.Interfaces.Services;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdString = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            var conversationIds = await _chatService.GetUserConversationIdsAsync(userId);
            foreach (var conversationId in conversationIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
            }
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}