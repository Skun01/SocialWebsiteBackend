using System;
using SocialWebsite.DTOs.User;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IUserService
{
    Task<Result<User>> GetUserByIdAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetAllUserAsync();
    Task<Result<User>> CreateUserAsync(CreateUserRequest request);
}
