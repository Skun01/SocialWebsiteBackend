using System;
using SocialWebsite.Entities;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Interfaces.Repositories;

public interface ILikeRepository
{
    public Task<Like> CreateLikeAsync(Guid userId, Guid TargetId, LikeType type);
    public Task DeleteLikeAsync(Guid likeId);
    public Task<IEnumerable<Like>> GetLikesByUserId(Guid userId);
    public Task<IEnumerable<Like>> GetLikesByTargetId(Guid targetId, LikeType type);
    public Task<int> GetTargetLikeNumber(Guid targetId, LikeType type);
    public Task<bool> IsUserLiked(Guid? userId, Guid targetId, LikeType type);
}
