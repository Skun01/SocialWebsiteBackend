using System;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsUserEmailExistAsync(string email);
    Task<bool> IsUserNameExistAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateVerifyEmailByIdAsync(Guid userId, bool IsEmailVerified);
    Task<bool> IsUserExistAsync(Guid userId);
    Task<PageList<UserResponse>> SearchAsync(UserQueryParameters query, Guid? currentUserId);
    Task UpdateUserRoleAsync(Guid userId, UserRole role);
}
