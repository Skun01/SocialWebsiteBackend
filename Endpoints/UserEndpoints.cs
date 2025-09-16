using System;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.User;
using SocialWebsite.Interfaces.Services;

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
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return Results.ValidationProblem(errors);
            }

            var result = await userService.CreateUserAsync(request);
            return result.IsSuccess ? Results.CreatedAtRoute("GetUserById", new { id = result.Value.Id }, result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IUserService userService
        ) =>
        {
            var result = await userService.GetUserByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }).WithName("GetUserById");

        group.MapGet("/", async (
            IUserService userService
        ) =>
        {
            var result = await userService.GetAllUserAsync();
            return Results.Ok(result.Value);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUserRequest request,
            IUserService userService,
            IValidator<UpdateUserRequest> validator
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                var result = await userService.UpdateUserAsync(id, request);
                return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
            }
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return Results.ValidationProblem(errors);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IUserService userService
        ) =>
        {
            var result = await userService.DeleteUserAsync(id);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result.Error);
        });

        group.MapPost("/users/{userId:guid}/avatar", async (
            Guid userId,
            IFormFile file,
            IUserService userService
        ) =>
        {
            var result = await userService.UploadUserAvatarAsync(userId, file);
            return result.IsSuccess
                ? Results.Ok(new { url = result.Value })
                : Results.BadRequest(result.Error);
        }).DisableAntiforgery();

        group.MapGet("/users/search", async (
            [AsParameters] UserQueryParameters query,
            IUserService userService,
            HttpContext context
        ) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? currentUserId = Guid.TryParse(userId, out var parsed) ? parsed : null;

            var result = await userService.SearchUserAsync(query, currentUserId);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
        return group;
    }
}
