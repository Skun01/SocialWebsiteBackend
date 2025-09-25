using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;

namespace SocialWebsite.Mapping;

public static class PostMapping
{
    public static PostResponse ToResponse(this Post post, string baseUrl, int likeCount, bool isLikeByMe)
    {
        return new PostResponse(
            Id: post.Id,
            AuthorId: post.User.Id,
            AuthorName: post.User.Username,
            AuthorAvatar: baseUrl + post.User.ProfilePictureUrl ?? "",
            Content: post.Content,
            Privacy: post.Privacy,
            LikeCount: likeCount,
            IsLikedByMe: isLikeByMe,
            CommentCount: post.Comments.Count,
            Files: post.Files.Select(pf => pf.ToResponse(baseUrl)).ToList(),
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }

    public static Post ToEntity(this CreatePostRequest request, Guid currentUserId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId,
            Content = request.Content,
            Privacy = request.Privacy
        };
    }
}
