using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.Chat;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class ChatEndpoints
{
    public static RouteGroupBuilder MapChatEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Chat")
            .RequireAuthorization();

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
            return result.ToApiResponse("Conversation created successfully");
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
            if (result.IsSuccess)
            {
                var response = ApiResponse<object>.SuccessResponse(
                    result.Value,
                    "Message created successfully"
                );
                response.TraceId = Activity.Current?.Id;
                return Results.Ok(response);
            }
            return result.ToApiResponse();
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
            return result.ToCursorApiResponse("Messages retrieved successfully");
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
            return result.ToApiResponse("Conversations retrieved successfully");
        });

        return group;
    }
    
}
