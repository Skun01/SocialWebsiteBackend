using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string text)
    {
        var dto = new { text, serverAt = DateTime.UtcNow };
        await Clients.All.SendAsync("MessageReceived", dto);
    }
}