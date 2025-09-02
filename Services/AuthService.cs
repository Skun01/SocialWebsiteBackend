using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    {
        _userRepo = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<UserResponse>> GetCurrentUserLogin(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null)
            return Result.Failure<UserResponse>(new Error("400", "Token is invalid"));

        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Result.Failure<UserResponse>(new Error("400", "User id in token is invalid"));

        User? user = await _userRepo.GetByIdAsync(userId);
        if (user is null)
            return Result.Failure<UserResponse>(new Error("404", "User not found"));

        return Result.Success(user.ToResponse());
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        User? user = await _userRepo.GetByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<LoginResponse>(new Error("UserEmail.NotFound", "User email not found"));

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Result.Failure<LoginResponse>(new Error("UserPassword.NotMatch", "User password is not correct"));

        string accesToken = _tokenService.CreateAccessToken(user);
        return Result.Success(new LoginResponse(accesToken));
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        bool isEmailExist = await _userRepo.IsUserEmailExistAsync(request.Email);
        if (isEmailExist)
            return Result.Failure(new Error("UserEmail.Exist", "This email has been registed"));

        bool isUserNameExist = await _userRepo.IsUserNameExistAsync(request.UserName);
        if (isUserNameExist)
            return Result.Failure(new Error("UserName.Exist", "User name already exist, try another name!"));

        User newUser = request.ToEntity();
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);
        await _userRepo.AddAsync(newUser);
        return Result.Success();
    }
}
