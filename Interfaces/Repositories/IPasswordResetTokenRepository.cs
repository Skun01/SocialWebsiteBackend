using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
{
    Task MarkUsedAsync(PasswordResetToken token);
    Task DeleteAllTokenByUserId(Guid UserId, Guid exceptTokenId);
}
