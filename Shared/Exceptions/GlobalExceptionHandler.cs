using System;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Shared.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        
        var response = exception switch
        {
            BadHttpRequestException => ApiResponse.ErrorResponse(
                "Invalid request format", 
                new List<string> { "The request body is not valid" }, 
                400),
            JsonException => ApiResponse.ErrorResponse(
                "Invalid JSON format", 
                new List<string> { "The request body contains invalid JSON" }, 
                400),
            UnauthorizedAccessException => ApiResponse.ErrorResponse(
                "Unauthorized access", 
                new List<string> { "You are not authorized to access this resource" }, 
                401),
            ArgumentException argEx => ApiResponse.ErrorResponse(
                "Invalid argument", 
                new List<string> { argEx.Message }, 
                400),
            _ => ApiResponse.ErrorResponse(
                "An unexpected error occurred", 
                new List<string> { "Internal server error" }, 
                500)
        };

        response.TraceId = traceId;

        // Structured logging
        Log.ForContext("TraceId", traceId)
           .ForContext("Path", httpContext.Request.Path)
           .ForContext("Method", httpContext.Request.Method)
           .ForContext("StatusCode", response.StatusCode)
           .ForContext("UserAgent", httpContext.Request.Headers.UserAgent.ToString())
           .ForContext("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString())
           .ForContext("UserId", httpContext.User?.Identity?.Name)
           .Error(exception, "Unhandled exception occurred");
        
        httpContext.Response.StatusCode = response.StatusCode ?? 500;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(
            System.Text.Json.JsonSerializer.Serialize(response), 
            cancellationToken);
        
        return true;
    }
}
