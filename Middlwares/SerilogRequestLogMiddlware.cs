using System;
using System.Diagnostics;
using Serilog;

namespace SocialWebsite.Middlwares;

public class SerilogRequestLogMiddlware
{
    private readonly RequestDelegate _next;
    public SerilogRequestLogMiddlware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();

        Log.ForContext("path", context.Request.Path)
        .ForContext("status", context.Response.StatusCode)
        .ForContext("elapsedMs", sw.ElapsedMilliseconds)
        .Information("HTTP {Method} {Path} -> {Status} in {Elapsed} ms",
        context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
