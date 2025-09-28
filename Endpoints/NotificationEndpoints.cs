using System;
using System.Collections.Generic;
using SocialWebsite.DTOs.Notification;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;
using System.Diagnostics;

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
            return result.ToCursorApiResponse("Notifications retrieved successfully");
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
            return result.ToApiResponse("Unread notification count retrieved successfully");
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
            return result.ToApiResponse("Notification marked as read successfully");
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
            if (result.IsSuccess)
            {
                var response = ApiResponse<object>.SuccessResponse(
                    new { NotificationChangedNumber = result.Value },
                    "All notifications marked as read successfully"
                );
                response.TraceId = Activity.Current?.Id;
                return Results.Ok(response);
            }
            
            return result.ToApiResponse();
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
            return result.ToApiResponse("Notification deleted successfully");
        });
        
        return group;
    }
}
