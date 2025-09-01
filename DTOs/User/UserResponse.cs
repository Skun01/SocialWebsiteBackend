using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.User;

public record class UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    Gender Gender,
    string? ProfilePictureUrl,
    bool IsEmailVerified
);