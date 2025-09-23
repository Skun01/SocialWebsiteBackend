using SocialWebsite.DTOs.User;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Notification;

public record class NotificationResponse(
    Guid Id,
    UserResponse TriggeredByUser,
    NotificationType Type,
    string Message,
    string Link,
    bool IsRead,
    DateTime CreatedAt
);