namespace SocialWebsite.DTOs.User;

public record class LoginRequest(
    string Email,
    string Password
);
