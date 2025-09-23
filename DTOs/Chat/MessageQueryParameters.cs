using System;

namespace SocialWebsite.DTOs.Chat;

public class MessageQueryParameters
{
    public int PageSize { set; get; }
    public string? Cursor { set; get; }
}
