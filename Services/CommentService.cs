using System;
using SocialWebsite.DTOs.comment;
using SocialWebsite.DTOs.Comment;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly IPostRepository _postRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILikeRepository _likeRepo;
    private readonly INotificationService _notificationService;
    public CommentService(ICommentRepository commentRepository, IPostRepository postRepository,
        IUserRepository userRepository, ILikeRepository likeRepository, INotificationService notificationService)
    {
        _commentRepo = commentRepository;
        _postRepo = postRepository;
        _userRepo = userRepository;
        _likeRepo = likeRepository;
        _notificationService = notificationService;
    }

    public async Task<Result<CommentResponse>> CreateNewCommentAsync(Guid postId, Guid currentUserId,
        CreateCommentRequest request)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<CommentResponse>(new Error("Post.NotFound", "Post not found!"));

        Comment newComment = new()
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = currentUserId,
            ParentCommentId = null,
            Content = request.Content
        };
        await _commentRepo.AddAsync(newComment);

        // create notification
        await _notificationService.CreateNotificationAsync(
            recipientId: post.UserId,
            triggerId: currentUserId,
            type: NotificationType.NewCommentOnPost,
            targetId: newComment.Id
        );
        
        return Result.Success(newComment.ToResponse());
    }

    public async Task<Result<CommentResponse>> CreateReplyCommentAsync(Guid currentUserId, Guid rootCommentId, CreateCommentRequest request)
    {
        var rootComment = await _commentRepo.GetByIdAsync(rootCommentId);
        if (rootComment is null)
            return Result.Failure<CommentResponse>(new Error("RootComment.NotFound", "Root comment not found"));

        Comment newReplyComment = new()
        {
            Id = Guid.NewGuid(),
            PostId = rootComment.PostId,
            UserId = currentUserId,
            ParentCommentId = rootCommentId,
            Content = request.Content
        };
        await _commentRepo.AddAsync(newReplyComment);

        // create notification
        var post = await _postRepo.GetByIdAsync(rootComment.PostId);
        await _notificationService.CreateNotificationAsync(
            recipientId: post!.UserId,
            triggerId: currentUserId,
            type: NotificationType.NewCommentOnPost,
            targetId: newReplyComment.Id
        );

        return Result.Success(newReplyComment.ToResponse());
    }

    public async Task<Result> DeleteCommentByIdAsync(Guid currentUserId, Guid commentId)
    {
        var comment = await _commentRepo.GetByIdAsync(commentId);
        if (comment is null)
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

        var replies = await _commentRepo.GetReplyResponsesByCommentId(rootCommentId);
        return Result.Success(replies);
    }

    public async Task<Result<IEnumerable<CommentResponse>>> GetRootCommentByPostIdAsync(Guid postId)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<IEnumerable<CommentResponse>>(new Error("Post.NotFound", "Post not found!"));
    
        var rootComments = await _commentRepo.GetRootCommentResponsesByPostId(postId);
        return Result.Success(rootComments);
    }

    public async Task<Result> LikeCommentAsync(Guid commentId, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure(new Error("CreatePost.UserNotFount", "User not found"));

        bool isUserLiked = await _likeRepo.IsUserLiked(currentUserId, commentId, LikeType.Comment);
        if (isUserLiked)
            return Result.Failure(new Error("UserLikedComment", "User has liked this Comment"));

        await _likeRepo.CreateLikeAsync(currentUserId, commentId, LikeType.Comment);
        return Result.Success();
    }

    public async Task<Result> UnlikeCommentAsync(Guid commentId, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure(new Error("CreatePost.UserNotFount", "User not found"));

        bool isUserLiked = await _likeRepo.IsUserLiked(currentUserId, commentId, LikeType.Comment);
        if (!isUserLiked)
            return Result.Failure(new Error("UserUnLikedComment", "User still haven't like this Comment"));

        await _likeRepo.DeleteLikeAsync(currentUserId, commentId, LikeType.Comment);
        return Result.Success();
    }

    public async Task<Result> UpdateCommentByIdAsync(Guid commentId, Guid currentUserId, UpdateCommentRequest request)
    {
        var comment = await _commentRepo.GetByIdAsync(commentId);
        if (comment is null)
            return Result.Failure(new Error("Comment.NotFound", "Comment not found"));

        if (comment.UserId != currentUserId)
            return Result.Failure(new Error("User.NotAllow", "You don't have permision to update this comment"));

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepo.UpdateAsync(comment);
        return Result.Success();
    }
}
