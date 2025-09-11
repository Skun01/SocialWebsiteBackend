using System;

namespace SocialWebsite.Data.Repositories;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;


public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly SocialWebsiteContext _context;
    public PasswordResetTokenRepository(SocialWebsiteContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken> AddAsync(PasswordResetToken entity)
    {
        _context.PasswordResetTokens.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAllTokenByUserId(Guid userId, Guid exceptTokenId)
    {
        List<PasswordResetToken>? tokens = _context.PasswordResetTokens
            .Where(t => t.UserId == userId && t.Id != exceptTokenId)
            .ToList();
        foreach (PasswordResetToken token in tokens)
        {
            _context.PasswordResetTokens.Remove(token);
        }
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(PasswordResetToken entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PasswordResetToken>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PasswordResetToken?> GetByIdAsync(Guid id)
    {
        return await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Id == id);

    }

    public async Task MarkUsedAsync(PasswordResetToken token)
    {
        token.IsUsed = true;
        token.UpdatedAt = DateTime.UtcNow;
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(PasswordResetToken entity)
    {
        throw new NotImplementedException();
    }
}
