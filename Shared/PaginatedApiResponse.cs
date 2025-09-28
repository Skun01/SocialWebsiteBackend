using System;
using System.Collections.Generic;

namespace SocialWebsite.Shared;

public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public PaginationMetadata Pagination { get; set; } = null!;

    public static PaginatedApiResponse<T> SuccessResponse(
        IEnumerable<T> data, 
        int totalCount, 
        int pageNumber, 
        int pageSize, 
        string? message = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200,
            Pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1
            }
        };
    }
}

public class PaginationMetadata
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class CursorApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public CursorMetadata Cursor { get; set; } = null!;

    public static CursorApiResponse<T> SuccessResponse(
        IEnumerable<T> data,
        string? nextCursor,
        bool hasNextPage,
        string? message = null)
    {
        return new CursorApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200,
            Cursor = new CursorMetadata
            {
                NextCursor = nextCursor,
                HasNextPage = hasNextPage
            }
        };
    }
}

public class CursorMetadata
{
    public string? NextCursor { get; set; }
    public bool HasNextPage { get; set; }
}
