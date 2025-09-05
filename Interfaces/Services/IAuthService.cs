using System;
using SocialWebsite.DTOs.User;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result> RegisterAsync(RegisterRequest request, HttpContext context, string endpointName);
    Task<Result<UserResponse>> GetCurrentUserLoginAsync(HttpContext httpContext);
    Task<Result> VerifyEmailAsync(string token);
}
