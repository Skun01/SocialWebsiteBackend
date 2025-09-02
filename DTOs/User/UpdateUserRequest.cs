using System;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.User;

public record UpdateUserRequest(
    string UserName,
    string Email,
    string? Password,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    Gender Gender,
    string? ProfilePictureUrl
);
