using System;
using System.Text;

namespace SocialWebsite.Shared;

public static class CursorHelper
{
    public static string EncodeCursor(DateTime createdAt, Guid id)
    {
        string cursorString = $"{createdAt:O}|{id}";
        byte[] bytes = Encoding.UTF8.GetBytes(cursorString);
        return Convert.ToBase64String(bytes);
    }
    public static (DateTime CreatedAt, Guid Id)? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor))
            return null;

        try
        {
            byte[] bytes = Convert.FromBase64String(cursor);
            string decodedString = System.Text.Encoding.UTF8.GetString(bytes);
            
            var parts = decodedString.Split('|');
            if (parts.Length != 2) return null;

            var createdAt = DateTime.Parse(parts[0]).ToUniversalTime();
            var id = Guid.Parse(parts[1]);

            return (createdAt, id);
        }
        catch
        {
            return null;
        }
    }
}
