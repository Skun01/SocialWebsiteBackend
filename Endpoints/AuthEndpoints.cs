using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.User;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Authentication");

        group.MapPost("/login", async ([FromBody] LoginRequest request,
            IAuthService authService, IValidator<LoginRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                var result = await authService.LoginAsync(request);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
            }

            return Results.ValidationProblem(validationResult.ToDictionary());
        });

        group.MapPost("/register", async (RegisterRequest request, HttpContext httpContext,
            IAuthService authService, IValidator<RegisterRequest> Validator) =>
        {
            var validationResult = await Validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                var result = await authService.RegisterAsync(request, httpContext, "ConfirmEmailEndpoint");
                return result.IsSuccess ? Results.Ok("Register successful, please check email to verify") : Results.BadRequest(result.Error);
            }

            return Results.ValidationProblem(validationResult.ToDictionary());
        });

        group.MapGet("/confirm-email", async ([FromQuery] string token, IAuthService authService) =>
        {
            var result = await authService.VerifyEmailAsync(token);
            return result.IsSuccess ? Results.Ok("Verify email successfuly") : Results.BadRequest(result.Error);
        }).WithName("ConfirmEmailEndpoint");

        group.MapGet("/me", async (HttpContext context, IAuthService authService) =>
        {
            var result = await authService.GetCurrentUserLoginAsync(context);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }).RequireAuthorization();
        return group;
    }
}
