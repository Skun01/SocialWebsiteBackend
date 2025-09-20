using System;
using SocialWebsite.DTOs.comment;
using SocialWebsite.Entities;

namespace SocialWebsite.Mapping;

public static class CommentMapping
{
    public static CommentResponse ToResponse(this Comment comment)
    {
        return new(
            Id: comment.Id,
            PostId: comment.PostId,
            UserId: comment.UserId,
            ParentCommentId: comment.ParentCommentId,
            Content: comment.Content,
            CreatedAt: comment.CreatedAt,
            UpdatedAt: comment.UpdatedAt,
            LikeCount: comment.Likes.Count,
            ReplyCount: comment.Replies.Count
        );
    }
}
