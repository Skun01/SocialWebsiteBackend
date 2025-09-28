using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Services;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class PostEndpoints
{
    public static RouteGroupBuilder MapPostEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Post");

        group.MapGet("/", async (
            [AsParameters] PostQueryParameters query,
            IPostService postService
        ) =>
        {
            var result = await postService.GetPostsAsync(query);
            return result.ToCursorApiResponse("Posts retrieved successfully");
        });

        group.MapPost("/", async (
            [FromBody] CreatePostRequest request,
            IPostService postService,
            HttpContext context,
            IValidator<CreatePostRequest> validator
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationErrorResponse();

            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }
                
            var result = await postService.CreatePostAsync(request, (Guid)currentUserId);
            return result.ToApiResponse("Post created successfully");
        }).RequireAuthorization();

        group.MapGet("/{postId:guid}", async (
            Guid postId,
            IPostService postService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }

            var result = await postService.GetPostByIdAsync(postId, Guid.Parse(currentUserId));
            return result.ToApiResponse();
        });

        group.MapDelete("/{postId:guid}", async (
            Guid postId,
            IPostService postService
        ) =>
        {
            var result = await postService.DeletePostAsync(postId);
            return result.ToApiResponse("Post deleted successfully");
        });

        group.MapPut("/{postId:guid}/privacy", async (
            Guid postId,
            ChangePostPrivacyRequest request,
            IPostService postService
        ) =>
        {
            var result = await postService.UpdatePostPrivacyAsync(postId, request);
            return result.ToApiResponse("Post privacy updated successfully");
        });

        group.MapPut("/{postId:guid}", async (
            Guid postId,
            UpdatePostRequest request,
            IPostService postService,
            HttpContext context,
            IValidator<UpdatePostRequest> validator
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationErrorResponse();

            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                var unauthorizedResponse = ApiResponse<object>.UnauthorizedResponse("User not authenticated");
                unauthorizedResponse.TraceId = Activity.Current?.Id;
                return Results.Json(unauthorizedResponse, statusCode: 401);
            }
                
            var result = await postService.UpdatePostAsync(postId, request, Guid.Parse(currentUserId));
            return result.ToApiResponse("Post updated successfully");
        });

        group.MapPost("/{postId:guid}/files", async (
            Guid postId,
            [FromForm] IFormFileCollection files,
            IPostService postService
        ) =>
        {
            var result = await postService.AddPostFileAsync(postId, files);
            return result.ToApiResponse("Files added to post successfully");
        }).DisableAntiforgery();

        group.MapDelete("/{postId:guid}/files/{postFileId:guid}", async (
            Guid postId,
            Guid postFileId,
            IPostService postService) =>
        {
            var result = await postService.DeletePostFileAsync(postId, postFileId);
            return result.ToApiResponse("File removed from post successfully");
        });

        group.MapPost("/{postId:guid}/likes", async (
            Guid postId,
            IPostService postService,
            HttpContext context) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await postService.LikePostAsync(postId, Guid.Parse(userId!));
            return result.ToApiResponse("Post liked successfully");
        }).RequireAuthorization();

        group.MapDelete("/{postId:guid}/likes", async (
            Guid postId,
            IPostService postService,
            HttpContext ctx) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await postService.UnlikePostAsync(postId, Guid.Parse(userId!));
            return result.ToApiResponse("Post unliked successfully");
        }).RequireAuthorization();

        group.MapGet("/{postId:guid}/comments", async (
            Guid postId,
            ICommentService commentService
        ) =>
        {
            var result = await commentService.GetRootCommentByPostIdAsync(postId);
            return result.ToApiResponse("Comments retrieved successfully");
        });

        group.MapPost("/{postId:guid}/comments", async(
            Guid postId,
            [FromBody] CreateCommentRequest request,
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

            var result = await commentService.CreateNewCommentAsync(postId, currentUserId, request);
            return result.ToApiResponse("Comment created successfully");
        }).RequireAuthorization();

        return group;
    }
}
