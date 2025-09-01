using System;
using Microsoft.AspNetCore.Identity;
using SocialWebsite.Data;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher<User> _passwordhasher;
    public UserService(IUserRepository userRepo, IPasswordHasher<User> passwordHasher)
    {
        _userRepo = userRepo;
        _passwordhasher = passwordHasher;
    }

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request)
    {
        bool isEmailExist = await _userRepo.IsUserEmailExistAsync(request.Email);
        if (isEmailExist)
            return Result.Failure<UserResponse>(new Error("User.EmailExist", "User email has been registered"));

        bool isUserNameExist = await _userRepo.IsUserNameExistAsync(request.UserName);
        if(isUserNameExist)
            return Result.Failure<UserResponse>(new Error("User.UserNameExist", "User name has already exist"));
        
        User newUser = request.ToEntity();
        newUser.PasswordHash = _passwordhasher.HashPassword(newUser, request.Password);
        await _userRepo.AddAsync(newUser);
        return Result.Success(newUser.ToResponse());
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetAllUserAsync()
    {
        var users = await _userRepo.GetAllAsync();
        IEnumerable<UserResponse> usersResponse = users.Select(u => u.ToResponse()).ToList();
        return Result.Success(usersResponse);
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid id)
    {
        User? userTarget = await _userRepo.GetByIdAsync(id);
        if (userTarget is null)
            return Result.Failure<UserResponse>(new Error("User.NotFound", "User not found!"));

        return Result.Success(userTarget.ToResponse());
    }
}
