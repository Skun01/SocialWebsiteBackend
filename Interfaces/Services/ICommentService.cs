using System;
using SocialWebsite.DTOs.comment;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface ICommentService
{
    Task<Result<IEnumerable<CommentResponse>>> GetRootCommentByPostIdAsync(Guid postId);
    Task<Result<IEnumerable<CommentResponse>>> GetRepliesCommentByRootCommentIdAsync(Guid rootCommentId);
    Task<Result<CommentResponse>> CreateNewCommentAsync(Guid postId, Guid currentUserId, CreateCommentRequest request);
    Task<Result> DeleteCommentByIdAsync(Guid postId, Guid currentUserId, Guid commentId);
    Task<Result> UpdateCommentByIdAsync(Guid postId, Guid commentId, Guid currentUserId, UpdateCommentRequest request);
}
