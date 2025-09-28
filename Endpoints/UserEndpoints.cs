using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.User;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Extensions;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Users");

        group.MapPost( "/", async (
            [FromBody] CreateUserRequest request,
            IUserService userService,
            IValidator<CreateUserRequest> validator
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationErrorResponse();
            }

            var result = await userService.CreateUserAsync(request);
            return result.ToApiResponse("User created successfully");
        }).RequireAdmin();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IUserService userService
        ) =>
        {
            var result = await userService.GetUserByIdAsync(id);
            return result.ToApiResponse();
        }).WithName("GetUserById");

        group.MapGet("/", async (
            IUserService userService
        ) =>
        {
            var result = await userService.GetAllUserAsync();
            return result.ToApiResponse("Users retrieved successfully");
        }).RequireModerator();

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUserRequest request,
            IUserService userService,
            IValidator<UpdateUserRequest> validator
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationErrorResponse();
            }

            var result = await userService.UpdateUserAsync(id, request);
            return result.ToApiResponse("User updated successfully");
        }).RequireAdmin();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IUserService userService
        ) =>
        {
            var result = await userService.DeleteUserAsync(id);
            return result.ToApiResponse("User deleted successfully");
        }).RequireAdmin();

        group.MapPost("/{userId:guid}/avatar", async (
            Guid userId,
            IFormFile file,
            IUserService userService
        ) =>
        {
            var result = await userService.UploadUserAvatarAsync(userId, file);
            if (result.IsSuccess)
            {
                var response = ApiResponse<object>.SuccessResponse(
                    new { url = result.Value }, 
                    "Avatar uploaded successfully"
                );
                response.TraceId = Activity.Current?.Id;
                return Results.Ok(response);
            }
            
            return result.ToApiResponse();
        }).DisableAntiforgery().RequireAuthorization();

        group.MapGet("/search", async (
            [AsParameters] UserQueryParameters query,
            IUserService userService,
            HttpContext context
        ) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? currentUserId = Guid.TryParse(userId, out var parsed) ? parsed : null;

            var result = await userService.SearchUserAsync(query, currentUserId);
            return result.ToPaginatedApiResponse("Search completed successfully");
        });
        return group;
    }
}
