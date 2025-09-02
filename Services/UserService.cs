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

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        User? userTarget = await _userRepo.GetByIdAsync(id);
        if (userTarget is null)
            return Result.Failure(new Error("UserDelete.NotFound", "User not found"));
            
        await _userRepo.DeleteAsync(userTarget);
        return Result.Success();
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

    public async Task<Result> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        User? userTarget = await _userRepo.GetByIdAsync(id);
        if (userTarget is null)
            return Result.Failure(new Error("UserUpdate.NotFound", "User not found!"));

        bool isEmailExist = await _userRepo.IsUserEmailExistAsync(request.Email);
        if (isEmailExist && request.Email != userTarget.Email)
            return Result.Failure(new Error("User.EmailExist", "User email has been registered"));

        bool isUserNameExist = await _userRepo.IsUserNameExistAsync(request.UserName);
        if (isUserNameExist && request.UserName != userTarget.Username)
            return Result.Failure(new Error("User.UserNameExist", "User name has already exist"));

        // update fields
        userTarget.Username = request.UserName;
        userTarget.Email = request.Email;
        if (!string.IsNullOrEmpty(request.Password))
            userTarget.PasswordHash = _passwordhasher.HashPassword(userTarget, request.Password);

        userTarget.FirstName = request.FirstName;
        userTarget.LastName = request.LastName;
        userTarget.DateOfBirth = request.DateOfBirth;
        userTarget.Gender = request.Gender;
        userTarget.ProfilePictureUrl = request.ProfilePictureUrl;

        await _userRepo.UpdateAsync(userTarget);
        return Result.Success();
    }
}
