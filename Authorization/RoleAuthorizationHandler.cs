using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SocialWebsite.Shared.Enums;
using Serilog;

namespace SocialWebsite.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Lấy role claim từ JWT token
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
        
        Log.Information("Authorization check - Required: {RequiredRole}, User role claim: {UserRoleClaim}", 
            requirement.MinimumRole, roleClaim ?? "NULL");
        
        if (string.IsNullOrEmpty(roleClaim))
        {
            Log.Warning("Authorization failed - No role claim found");
            context.Fail();
            return Task.CompletedTask;
        }

        // Parse role từ string sang enum
        if (!Enum.TryParse<UserRole>(roleClaim, true, out var userRole))
        {
            Log.Warning("Authorization failed - Invalid role claim: {RoleClaim}", roleClaim);
            context.Fail();
            return Task.CompletedTask;
        }

        // Kiểm tra xem user có role đủ cao không
        if (userRole >= requirement.MinimumRole)
        {
            Log.Information("Authorization succeeded - User role {UserRole} >= Required {RequiredRole}", 
                userRole, requirement.MinimumRole);
            context.Succeed(requirement);
        }
        else
        {
            Log.Warning("Authorization failed - User role {UserRole} < Required {RequiredRole}", 
                userRole, requirement.MinimumRole);
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
