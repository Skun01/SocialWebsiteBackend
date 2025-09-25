using System;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Endpoints;

public static class NotificationEndpoints
{
    public static RouteGroupBuilder MapNotificationEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Notifications")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] NotificationQueryParameters query,
            INotificationService notificationService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await notificationService.GetUserNotificationsAsync((Guid)currentUserId, query);
            return Results.Ok(result.Value);
        });

        group.MapGet("/unread", async (
            INotificationService notificationService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await notificationService.GetUnreadNotificationCountAsync((Guid)currentUserId);
            return Results.Ok(result.Value);
        });

        group.MapPut("/{notificationId:guid}", async (
            Guid notificationId,
            INotificationService notificationService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await notificationService.MarkNotificationAsReadAsync(notificationId, (Guid)currentUserId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapPut("/read-all", async (
            INotificationService notificationService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await notificationService.MarkAllNotificationsAsReadAsync((Guid)currentUserId);
            return Results.Ok(new {NotificationChangedNumber = result.Value});
        });

        group.MapDelete("/{notificationId:guid}", async (
            Guid notificationId,
            INotificationService notificationService,
            HttpContext context
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await notificationService.DeleteNotificationAsync(notificationId, (Guid)currentUserId);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });
        
        return group;
    }
}
