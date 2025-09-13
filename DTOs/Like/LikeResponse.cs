namespace SocialWebsite.DTOs.Like;

public record class LikeResponse(
    Guid UserLikedId,
    string UserLikedName
);