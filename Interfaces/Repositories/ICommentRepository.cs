using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetRootCommentsByPostId(Guid postId);
    Task<IEnumerable<Comment>> GetRepliesByCommentId(Guid parentCommentId);
}
