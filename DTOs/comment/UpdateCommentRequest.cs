using System;

namespace SocialWebsite.DTOs.Comment;

public record class UpdateCommentRequest(
    string Content
);
