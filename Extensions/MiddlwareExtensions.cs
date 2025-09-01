using System;
using SocialWebsite.Middlwares;

namespace SocialWebsite.Extensions;

public static class MiddlwareExtensions
{
    public static IApplicationBuilder UseSeriRequestLog(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SerilogRequestLogMiddlware>();
    }
}
