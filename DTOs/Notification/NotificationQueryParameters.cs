using System;

namespace SocialWebsite.DTOs.Notification;

public class NotificationQueryParameters
{
    public int PageSize { set; get; }
    public string? Cursor { set; get; }
}
