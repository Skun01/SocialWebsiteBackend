using System;
using SocialWebsite.DTOs.comment;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetRootCommentsByPostId(Guid postId);
    Task<IEnumerable<Comment>> GetRepliesByCommentId(Guid parentCommentId);
    Task<IEnumerable<CommentResponse>> GetRootCommentResponsesByPostId(Guid postId);
    Task<IEnumerable<CommentResponse>> GetReplyResponsesByCommentId(Guid parentCommentId);
}
