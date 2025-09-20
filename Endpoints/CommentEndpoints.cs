using System;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.Entities;
using System.Security.Claims;

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
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
        group.MapPost("/{commentId:guid}/repliess", async (
            Guid commentId,
            ICommentService commentService,
            CreateCommentRequest request,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
                return Results.BadRequest("User Id not found");

            var result = await commentService.CreateReplyCommentAsync(currentUserId, commentId, request);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapDelete("/{commentId:guid}", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
                return Results.BadRequest("User Id not found");

            var result = await commentService.DeleteCommentByIdAsync(currentUserId, commentId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
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
                return Results.BadRequest("User Id not found");

            var result = await commentService.UpdateCommentByIdAsync(commentId, currentUserId, request);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization();

        group.MapPost("/{commentId:guid}/likes", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await commentService.LikeCommentAsync(commentId, Guid.Parse(userId!));
            return result.IsSuccess ? Results.NoContent()
                                    : Results.BadRequest(result.Error);
        }).RequireAuthorization();

        group.MapDelete("/{commentId:guid}/likes", async (
            Guid commentId,
            ICommentService commentService,
            HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await commentService.UnlikeCommentAsync(commentId, Guid.Parse(userId!));
            return result.IsSuccess ? Results.NoContent()
                                    : Results.BadRequest(result.Error);
        }).RequireAuthorization();
        
        return group;
    }
}
