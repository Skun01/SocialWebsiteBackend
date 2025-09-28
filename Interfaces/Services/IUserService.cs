using System;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserResponse>> GetUserByIdAsync(Guid id);
    Task<Result<IEnumerable<UserResponse>>> GetAllUserAsync();
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request);
    Task<Result> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<Result> DeleteUserAsync(Guid id);
    Task<Result<string>> UploadUserAvatarAsync(Guid userId, IFormFile file);
    Task<Result<PageList<UserResponse>>> SearchUserAsync(UserQueryParameters query, Guid? currentUserId);
    Task<Result> SetUserRoleAsync(Guid userId, UserRole role);
}
