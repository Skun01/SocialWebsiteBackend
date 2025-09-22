using System;
using System.Security.Claims;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Endpoints;

public static class ChatEndpoints
{
    public static RouteGroupBuilder MapChatEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Chat");

        group.MapPost("/conversations", async (
            CreateConversationRequest request,
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var creatorUserId = GetCurrentUserId(context); 
            var conversation = await chatService.CreateConversationAsync(creatorUserId, request.RecipientUserId);

            return Results.Ok(conversation);
        });

        group.MapPost("/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            CreateMessageRequest request,
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var senderUserId = GetCurrentUserId(context);
            try
            {
                var message = await chatService.CreateMessageAsync(conversationId, senderUserId, request.Content);
                return Results.Created($"/api/chat/messages/{message.Id}", message);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid(); 
            }
        });



        return group;
    }
    private static Guid GetCurrentUserId(HttpContext httpContext)
        {
            var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }
            throw new Exception("Không thể xác định người dùng.");
        }
}
