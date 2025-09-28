using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.User;

public record class SetUserRoleRequest(
    UserRole Role
);
