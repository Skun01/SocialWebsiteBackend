using SocialWebsite.Shared;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SocialWebsite.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResponse<T>(this Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<T>.SuccessResponse(result.Value, successMessage);
            response.TraceId = Activity.Current?.Id;
            return Results.Ok(response);
        }

        var errorResponse = ApiResponse<T>.ErrorResponse(
            result.Error.Description, 
            new List<string> { result.Error.Code },
            GetStatusCodeFromError(result.Error.Code)
        );
        errorResponse.TraceId = Activity.Current?.Id;
        
        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }

    public static IResult ToApiResponse(this Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse.SuccessResponse(successMessage);
            response.TraceId = Activity.Current?.Id;
            return Results.Ok(response);
        }

        var errorResponse = ApiResponse.ErrorResponse(
            result.Error.Description,
            new List<string> { result.Error.Code },
            GetStatusCodeFromError(result.Error.Code)
        );
        errorResponse.TraceId = Activity.Current?.Id;

        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }

    public static IResult ToPaginatedApiResponse<T>(
        this Result<PageList<T>> result, 
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var pageList = result.Value;
            var response = PaginatedApiResponse<T>.SuccessResponse(
                pageList.Items,
                pageList.TotalCount,
                pageList.PageNumber,
                pageList.PageSize,
                successMessage
            );
            response.TraceId = Activity.Current?.Id;
            return Results.Ok(response);
        }

        var errorResponse = ApiResponse<IEnumerable<T>>.ErrorResponse(
            result.Error.Description,
            new List<string> { result.Error.Code },
            GetStatusCodeFromError(result.Error.Code)
        );
        errorResponse.TraceId = Activity.Current?.Id;

        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }

    public static IResult ToCursorApiResponse<T>(
        this Result<CursorList<T>> result,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var cursorList = result.Value;
            var response = CursorApiResponse<T>.SuccessResponse(
                cursorList.Items,
                cursorList.NextCursor,
                cursorList.HasNextPage,
                successMessage
            );
            response.TraceId = Activity.Current?.Id;
            return Results.Ok(response);
        }

        var errorResponse = ApiResponse<IEnumerable<T>>.ErrorResponse(
            result.Error.Description,
            new List<string> { result.Error.Code },
            GetStatusCodeFromError(result.Error.Code)
        );
        errorResponse.TraceId = Activity.Current?.Id;

        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }

    public static IResult ToValidationErrorResponse(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => e.ErrorMessage)
            .ToList();
        
        var errorResponse = ApiResponse.ErrorResponse(
            "Validation failed", 
            errors, 
            422
        );
        errorResponse.TraceId = Activity.Current?.Id;
        return Results.Json(errorResponse, statusCode: 422);
    }

    private static int GetStatusCodeFromError(string errorCode)
    {
        return errorCode switch
        {
            var code when code.Contains("NotFound") => 404,
            var code when code.Contains("Unauthorized") => 401,
            var code when code.Contains("Forbidden") => 403,
            var code when code.Contains("Validation") => 422,
            var code when code.Contains("Conflict") => 409,
            var code when code.Contains("EmailExist") => 409,
            var code when code.Contains("UserNameExist") => 409,
            _ => 400
        };
    }
}
