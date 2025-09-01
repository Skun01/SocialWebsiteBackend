using System;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.User;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Users");

        group.MapPost("/", async ([FromBody] CreateUserRequest request,
            IUserService userService, IValidator<CreateUserRequest> validator) =>
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
            return result.IsSuccess ? Results.CreatedAtRoute("GetUserById", new{id = result.Value.Id}, result.Value) 
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/{id}", async (Guid id, IUserService userService) =>
        {
            var result = await userService.GetUserByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }).WithName("GetUserById");

        group.MapGet("/", async (IUserService userService) =>
        {
            var result = await userService.GetAllUserAsync();
            return Results.Ok(result.Value);
        });

        return group;
    }
}
