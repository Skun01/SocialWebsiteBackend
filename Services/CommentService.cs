using System;
using SocialWebsite.DTOs.comment;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly IPostRepository _postRepo;
    public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
    {
        _commentRepo = commentRepository;
        _postRepo = postRepository;
    }

    public async Task<Result<CommentResponse>> CreateNewCommentAsync(Guid postId, Guid currentUserId, CreateCommentRequest request)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<CommentResponse>(new Error("Post.NotFound", "Post not found!"));


        Comment newComment = new()
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = currentUserId,
            ParentCommentId = request.ParentCommentId,
            Content = request.Content
        };
        await _commentRepo.AddAsync(newComment);
        return Result.Success(newComment.ToResponse());
    }

    public async Task<Result> DeleteCommentByIdAsync(Guid postId, Guid currentUserId, Guid commentId)
    {
        var comment = await _commentRepo.GetByIdAsync(commentId);
        if (comment is null || comment.PostId != postId)
            return Result.Failure(new Error("Comment.NotFound", "Comment not found"));
        
        if (comment.UserId != currentUserId)
            return Result.Failure(new Error("User.NotAllow", "You don't have permision to delete this comment"));

        await _commentRepo.DeleteAsync(comment);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<CommentResponse>>> GetRepliesCommentByRootCommentIdAsync(Guid rootCommentId)
    {
        var rootComent = await _commentRepo.GetByIdAsync(rootCommentId);
        if (rootComent is null)
            return Result.Failure<IEnumerable<CommentResponse>>(new Error("RootComment.NotFound", "Root comment not found"));

        var replies = await _commentRepo.GetRepliesByCommentId(rootCommentId);
        var response = replies.Select(c => c.ToResponse());
        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<CommentResponse>>> GetRootCommentByPostIdAsync(Guid postId)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<IEnumerable<CommentResponse>>(new Error("Post.NotFound", "Post not found!"));
    
        IEnumerable<Comment> rootComments = await _commentRepo.GetRootCommentsByPostId(postId);
        var response = rootComments.Select(c => c.ToResponse());
        return Result.Success(response);
    }

    public async Task<Result> UpdateCommentByIdAsync(Guid postId, Guid commentId, Guid currentUserId, UpdateCommentRequest request)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure(new Error("Post.NotFound", "Post not found!"));

        var comment = await _commentRepo.GetByIdAsync(commentId);
        if (comment is null || comment.PostId != postId)
            return Result.Failure(new Error("Comment.NotFound", "Comment not found"));

        if (comment.UserId != currentUserId)
            return Result.Failure(new Error("User.NotAllow", "You don't have permision to update this comment"));

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepo.UpdateAsync(comment);
        return Result.Success();
    }
}
