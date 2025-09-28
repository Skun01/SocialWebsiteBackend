using System;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Mapping;

public static class UserMapping
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
            Id: user.Id,
            UserName: user.Username,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender,
            ProfilePictureUrl: user.ProfilePictureUrl,
            IsEmailVerified: user.IsEmailVerified,
            Role: user.Role
        );
    }

    public static User ToEntity(this CreateUserRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = request.UserName,
            Email = request.Email,
            PasswordHash = "",
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            ProfilePictureUrl = request.ProfilePictureUrl,
            IsEmailVerified = false,
            IsActive = false,
            Role = UserRole.User
        };
    }

    public static User ToEntity(this RegisterRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = request.UserName,
            Email = request.Email,
            PasswordHash = "",
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            ProfilePictureUrl = request.ProfilePictureUrl,
            IsEmailVerified = false,
            IsActive = false,
            Role = UserRole.User
        };
    }
}
