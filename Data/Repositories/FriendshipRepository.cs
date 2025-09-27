using Microsoft.EntityFrameworkCore;
using SocialWebsite.Data;
using SocialWebsite.DTOs.Friendship;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly SocialWebsiteContext _context;

    public FriendshipRepository(SocialWebsiteContext context)
    {
        _context = context;
    }

    public async Task<Friendship> AddAsync(Friendship entity)
    {
        await _context.Friendships.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> AreFriendsAsync(Guid userId1, Guid userId2)
    {
        return await _context.Friendships
            .AnyAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.SenderId == userId1 && f.ReceiverId == userId2) ||
                 (f.SenderId == userId2 && f.ReceiverId == userId1)));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var friendship = await _context.Friendships.FindAsync(id);
        if (friendship == null) return false;

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Friendship>> GetAllAsync()
    {
        return await _context.Friendships.ToListAsync();
    }

    public async Task<Friendship?> GetByIdAsync(Guid id)
    {
        return await _context.Friendships.FindAsync(id);
    }

    public async Task<Friendship?> GetFriendshipAsync(Guid senderId, Guid receiverId, FriendshipStatus status)
    {
        return await _context.Friendships
            .FirstOrDefaultAsync(
                f => f.SenderId == senderId
                && f.ReceiverId == receiverId
                && f.Status == status
            );
    }

    public async Task<FriendshipStatus?> GetFriendshipStatusAsync(Guid userId1, Guid userId2)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.SenderId == userId1 && f.ReceiverId == userId2) ||
                (f.SenderId == userId2 && f.ReceiverId == userId1));

        return friendship?.Status;
    }

    public async Task<IEnumerable<FriendshipResponse>> GetFriendsAsync(Guid userId)
    {
        var friends = await _context.Friendships
            .AsNoTracking()
            .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted)
            .Select(f => new
            {
                User = f.SenderId == userId ? f.Receiver : f.Sender,
                FriendSince = f.UpdatedAt ?? f.CreatedAt
            })
            .Select(x => new FriendshipResponse(
                x.User.Id,
                x.User.Username,
                x.User.ProfilePictureUrl,
                x.FriendSince,
                FriendshipStatus.Accepted
            ))
            .ToListAsync();

        return friends;
    }

    public async Task<IEnumerable<FriendshipResponse>> GetReceivedFriendRequestsAsync(Guid userId)
    {
        var requests = await _context.Friendships
            .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Sender)
            .AsNoTracking()
            .Select(x => new FriendshipResponse(
                x.Sender.Id,
                x.Sender.Username,
                x.Sender.ProfilePictureUrl,
                x.CreatedAt,
                x.Status
            ))
            .ToListAsync();

        return requests;
    }

    public async Task<IEnumerable<FriendshipResponse>> GetSentFriendRequestsAsync(Guid userId)
    {
        var requests = await _context.Friendships
            .Where(f => f.SenderId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Receiver)
            .AsNoTracking()
            .Select(x => new FriendshipResponse(
                x.Receiver.Id,
                x.Receiver.Username,
                x.Receiver.ProfilePictureUrl,
                x.CreatedAt,
                x.Status
            ))
            .ToListAsync();

        return requests;
    }

    public async Task<Friendship> UpdateAsync(Friendship entity)
    {
        _context.Friendships.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

}