using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialWebsite.DTOs.Auth;
using SocialWebsite.DTOs.User;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Extensions;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Authentication");

        group.MapPost("/login", async ([FromBody] LoginRequest request,
            IAuthService authService, IValidator<LoginRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationErrorResponse();
            }

            var result = await authService.LoginAsync(request);
            return result.ToApiResponse("Login successful");
        });

        group.MapPost("/register", async (RegisterRequest request, HttpContext httpContext,
            IAuthService authService, IValidator<RegisterRequest> Validator) =>
        {
            var validationResult = await Validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationErrorResponse();
            }

            var result = await authService.RegisterAsync(request, httpContext, "ConfirmEmailEndpoint");
            return result.ToApiResponse("Registration successful, please check email to verify");
        });

        group.MapGet("/confirm-email", async ([FromQuery] string token, IAuthService authService) =>
        {
            var result = await authService.VerifyEmailAsync(token);
            return result.ToApiResponse("Email verified successfully");
        }).WithName("ConfirmEmailEndpoint");

        group.MapGet("/me", async (HttpContext context, IAuthService authService) =>
        {
            var result = await authService.GetCurrentUserLoginAsync(context);
            return result.ToApiResponse();
        }).RequireAuthorization();

        group.MapPost("/forgot-password", async (ForgotPasswordRequest request, IAuthService authService) =>
        {
            var result = await authService.SendResetPasswordEMailAsync(request);
            return result.ToApiResponse("Password reset email sent");
        });

        group.MapPost("/reset-password", async (ResetPasswordRequest request, IAuthService authService) =>
        {
            var result = await authService.ResetPasswordAsync(request.PublicId, request.Token, request.NewPassword);
            return result.ToApiResponse("Password has been reset successfully");
        });
        
        return group;
    }
}
