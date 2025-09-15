using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Services;
using SocialWebsite.Shared;

namespace SocialWebsite.Endpoints;

public static class PostEndpoints
{
    public static RouteGroupBuilder MapPostEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Post");

        group.MapGet("/", async (IPostService postService, HttpContext context) =>
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
                return Results.BadRequest("Validate token is invalid");

            var result = await postService.GetAllPostAsync(Guid.Parse(currentUserId));
            return Results.Ok(result.Value);
        }).RequireAuthorization();

        group.MapPost("/", async ([FromBody] CreatePostRequest request, IPostService postService) =>
        {
            var result = await postService.CreatePostAsync(request);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/{postId:guid}", async (Guid postId, IPostService postService, HttpContext context) =>
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
                return Results.BadRequest("Validate token is invalid");

            var result = await postService.GetPostByIdAsync(postId, Guid.Parse(currentUserId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapDelete("/{postId:guid}", async ([FromRoute] Guid postId, IPostService postService) =>
        {
            var result = await postService.DeletePostAsync(postId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapPut("/{postId:guid}/privacy", async ([FromRoute] Guid postId,
            ChangePostPrivacyRequest request, IPostService postService) =>
        {
            var result = await postService.UpdatePostPrivacyAsync(postId, request);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapPut("/{postId:guid}", async ([FromRoute] Guid postId,
            UpdatePostRequest request, IPostService postService, HttpContext context) =>
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
                return Results.BadRequest("Validate token is invalid");
                
            var result = await postService.UpdatePostAsync(postId, request, Guid.Parse(currentUserId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/{postId:guid}/files", async ([FromRoute] Guid postId, [FromForm] IFormFileCollection files, IPostService postService) =>
        {
            var result = await postService.AddPostFileAsync(postId, files);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).DisableAntiforgery();

        group.MapDelete("/{postId:guid}/files/{postFileId:guid}", async (Guid postId, Guid postFileId, IPostService postService) =>
        {
            var result = await postService.DeletePostFileAsync(postId, postFileId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/{postId:guid}/likes", async (Guid postId, IPostService postService, HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await postService.LikePostAsync(postId, Guid.Parse(userId!));
            return result.IsSuccess ? Results.NoContent()
                                    : Results.BadRequest(result.Error);
        }).RequireAuthorization();

        group.MapDelete("/{postId:guid}/likes", async (Guid postId, IPostService postService, HttpContext ctx) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await postService.UnlikePostAsync(postId, Guid.Parse(userId!));
            return result.IsSuccess ? Results.NoContent()
                                    : Results.BadRequest(result.Error);
        }).RequireAuthorization();


        return group;
    }
}
