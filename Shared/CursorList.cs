using System;

namespace SocialWebsite.Shared;

public class CursorList<T>
{
    public List<T> Items { get; private set; }
    public string? NextCursor { get; private set; }
    public bool HasNextPage { get; private set; }
    public CursorList(List<T> items, string? nextCursor, bool hasNextPage)
    {
        Items = items;
        NextCursor = nextCursor;
        HasNextPage = hasNextPage;
    }
}
