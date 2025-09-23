using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            var result = await chatService.CreateConversationAsync(creatorUserId, request);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            CreateMessageRequest request,
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var senderUserId = GetCurrentUserId(context);
            var result = await chatService.CreateMessageAsync(conversationId, senderUserId, request);
            return result.IsSuccess
                ? Results.Created($"/api/chat/messages/{result.Value.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            [AsParameters] MessageQueryParameters query,
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var currentUserId = GetCurrentUserId(context);
            var result = await chatService.GetConversationMessagesAsync(conversationId, currentUserId, query);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/conversations", async (
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var currentUserId = GetCurrentUserId(context);
            var result = await chatService.GetUserConversationsAsync(currentUserId);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
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
