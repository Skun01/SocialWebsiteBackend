using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Lấy role claim từ JWT token
        var roleClaim = context.User.FindFirst("role")?.Value;
        
        if (string.IsNullOrEmpty(roleClaim))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Parse role từ string sang enum
        if (!Enum.TryParse<UserRole>(roleClaim, true, out var userRole))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Kiểm tra xem user có role đủ cao không
        if (userRole >= requirement.MinimumRole)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
