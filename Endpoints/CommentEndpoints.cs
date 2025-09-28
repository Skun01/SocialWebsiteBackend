using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.Entities;
using System.Security.Claims;
using SocialWebsite.Extensions;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class CommentEndpoints
{
    public static RouteGroupBuilder MapCommentEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Comments");

        group.MapGet("/{commentId:guid}/replies", async (
            Guid commentId,
            ICommentService commentService
        ) =>
        {
            var result = await commentService.GetRepliesCommentByRootCommentIdAsync(commentId);
            return result.ToApiResponse("Replies retrieved successfully");
        });
        group.MapPost("/{commentId:guid}/replies", async (
            Guid commentId,
            ICommentService commentService,
            CreateCommentRequest request,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }

            var result = await commentService.CreateReplyCommentAsync(currentUserId, commentId, request);
            return result.ToApiResponse("Reply created successfully");
        }).RequireAuthorization();

        group.MapDelete("/{commentId:guid}", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }

            var result = await commentService.DeleteCommentByIdAsync(currentUserId, commentId);
            return result.ToApiResponse("Comment deleted successfully");
        }).RequireAuthorization();

        group.MapPut("/{commentId:guid}", async (
            Guid commentId,
            [FromBody] UpdateCommentRequest request,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }

            var result = await commentService.UpdateCommentByIdAsync(commentId, currentUserId, request);
            return result.ToApiResponse("Comment updated successfully");
        }).RequireAuthorization();

        group.MapPost("/{commentId:guid}/likes", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await commentService.LikeCommentAsync(commentId, Guid.Parse(userId!));
            return result.ToApiResponse("Comment liked successfully");
        }).RequireAuthorization();

        group.MapDelete("/{commentId:guid}/likes", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await commentService.UnlikeCommentAsync(commentId, Guid.Parse(userId!));
            return result.ToApiResponse("Comment unliked successfully");
        }).RequireAuthorization();
        
        return group;
    }
}
