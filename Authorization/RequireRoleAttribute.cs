using Microsoft.AspNetCore.Authorization;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(UserRole minimumRole)
    {
        Policy = $"RequireRole_{minimumRole}";
    }
}
