using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Chat;

public record class ParticipantResponse(
    Guid UserId,
    string UserName,
    ParticipantRole Role,
    string AvatarUrl
);
