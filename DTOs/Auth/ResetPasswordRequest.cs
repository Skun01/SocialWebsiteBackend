namespace SocialWebsite.DTOs.Auth;

public record class ResetPasswordRequest(Guid PublicId, string Token, string NewPassword);
