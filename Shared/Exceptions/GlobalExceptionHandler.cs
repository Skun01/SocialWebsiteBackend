using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace SocialWebsite.Shared.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetail = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "System error",
            Detail = "An unexpected error occured.",
            Instance = httpContext.Request.Path
        };
        var traceId = httpContext.TraceIdentifier;
        problemDetail.Extensions["traceId"] = traceId;
        Log.ForContext("traceId", traceId)
           .ForContext("path", httpContext.Request.Path)
           .Error(exception, "unhandled exception");

        httpContext.Response.StatusCode = problemDetail.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);
        return true;

    }
}
