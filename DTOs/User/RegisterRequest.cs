using SocialWebsite.Shared.Enums;
namespace SocialWebsite.DTOs.User;

public record class RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    Gender Gender,
    string? ProfilePictureUrl
);