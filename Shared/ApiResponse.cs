using System;
using System.Collections.Generic;

namespace SocialWebsite.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> NotFoundResponse(string message = "Resource not found")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = 404
        };
    }

    public static ApiResponse<T> UnauthorizedResponse(string message = "Unauthorized access")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = 401
        };
    }
}

// Non-generic version cho responses không có data
public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = 200
        };
    }

    public static ApiResponse ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}
