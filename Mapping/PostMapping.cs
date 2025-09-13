using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;

namespace SocialWebsite.Mapping;

public static class PostMapping
{
    public static PostResponse ToResponse(this Post post, string baseUrl)
    {
        return new PostResponse(
            Id: post.Id,
            AuthorId: post.User.Id,
            AuthorName: post.User.Username,
            AuthorAvatar: baseUrl + post.User.ProfilePictureUrl ?? "",
            Content: post.Content,
            Privacy: post.Privacy,
            LikeCount: post.Likes.Count,
            CommentCount: post.Comments.Count,
            Files: [.. post.Files.Select(f => f.ToResponse(baseUrl))],
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }

    public static Post ToEntity(this CreatePostRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Content = request.Content,
            Privacy = request.Privacy
        };
    }
}
