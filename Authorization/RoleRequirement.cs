using Microsoft.AspNetCore.Authorization;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Authorization;

/// <summary>
/// Requirement để kiểm tra role của user
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public UserRole MinimumRole { get; }

    public RoleRequirement(UserRole minimumRole)
    {
        MinimumRole = minimumRole;
    }
}
