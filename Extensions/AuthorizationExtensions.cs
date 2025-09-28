using Microsoft.AspNetCore.Authorization;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Extensions;

public static class AuthorizationExtensions
{
    // Yêu cầu user phải đăng nhập
    public static TBuilder RequireAuthentication<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization();
    }

    // Yêu cầu role tối thiểu
    public static TBuilder RequireRole<TBuilder>(this TBuilder builder, UserRole minimumRole)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization($"RequireRole_{minimumRole}");
    }

    // Yêu cầu role User (default role)
    public static TBuilder RequireUser<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireRole(UserRole.User);
    }

    //Yêu cầu role Moderator trở lên
    public static TBuilder RequireModerator<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireRole(UserRole.Moderator);
    }

    // Yêu cầu role Admin trở lên
    public static TBuilder RequireAdmin<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireRole(UserRole.Admin);
    }

}
