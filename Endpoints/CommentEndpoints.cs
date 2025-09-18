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

        group.MapGet("/", async (
            Guid postId,
            ICommentService commentService
        ) =>
        {
            var result = await commentService.GetRootCommentByPostIdAsync(postId);
            return Results.Ok(result.Value);
        });

        group.MapGet("/{rootCommentId:guid}", async (
            Guid rootCommentId,
            ICommentService commentService
        ) =>
        {
            var result = await commentService.GetRepliesCommentByRootCommentIdAsync(rootCommentId);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/", async(
            Guid postId,
            [FromBody] CreateCommentRequest request,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
                return Results.BadRequest("User Id not found");

            var result = await commentService.CreateNewCommentAsync(postId, currentUserId, request);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization();

        group.MapDelete("/{commentId:guid}", async (
            Guid postId,
            Guid commentId,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
                return Results.BadRequest("User Id not found");

            var result = await commentService.DeleteCommentByIdAsync(postId, currentUserId, commentId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization();

        group.MapPut("/{commentId:guid}", async (
            Guid postId,
            Guid commentId,
            [FromBody] UpdateCommentRequest request,
            ICommentService commentService,
            HttpContext httpContext
        ) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !Guid.TryParse(userId.ToString(), out Guid currentUserId))
                return Results.BadRequest("User Id not found");

            var result = await commentService.UpdateCommentByIdAsync(postId, commentId, currentUserId, request);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization();
        
        return group;
    }
}
