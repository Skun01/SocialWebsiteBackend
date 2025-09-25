using System;
using System.Security.Claims;

namespace SocialWebsite.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetCurrentUserId(this HttpContext httpContext)
    {
        var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            return userId;
        }
        return null;
    }
}
