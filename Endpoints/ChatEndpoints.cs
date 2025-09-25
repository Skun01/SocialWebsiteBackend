using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Extensions;
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
            var creatorUserId = context.GetCurrentUserId();
            if(creatorUserId is null)
                return Results.Unauthorized();

            var result = await chatService.CreateConversationAsync((Guid)creatorUserId, request);
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
            var senderUserId = context.GetCurrentUserId();
            if(senderUserId is null)
                return Results.Unauthorized();
            var result = await chatService.CreateMessageAsync(conversationId, (Guid)senderUserId, request);
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
            var currentUserId = context.GetCurrentUserId();
            if(currentUserId is null)
                return Results.Unauthorized();
            var result = await chatService.GetConversationMessagesAsync(conversationId, (Guid)currentUserId, query);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/conversations", async (
            IChatService chatService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if(currentUserId is null)
                return Results.Unauthorized();
            var result = await chatService.GetUserConversationsAsync((Guid)currentUserId);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        return group;
    }
    
}
