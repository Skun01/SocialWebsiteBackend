using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Chat;

public record class ParticipantResponse(
    Guid UserId,
    ParticipantRole Role,
    DateTime JoinedAt
);
