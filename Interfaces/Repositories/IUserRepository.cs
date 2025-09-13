using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsUserEmailExistAsync(string email);
    Task<bool> IsUserNameExistAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateVerifyEmailByIdAsync(Guid userId, bool IsEmailVerified);
    Task<bool> IsUserExistAsync(Guid userId);
}
