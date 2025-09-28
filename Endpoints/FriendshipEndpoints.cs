using System;
using System.Collections.Generic;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;
using System.Diagnostics;

namespace SocialWebsite.Endpoints;

public static class FriendshipEndpoints
{
    public static RouteGroupBuilder MapFriendShipEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Friendships")
            .RequireAuthorization();

        group.MapGet("/", async (
            HttpContext context,
            IFriendshipService friendshipService
        ) =>
        {
            var userId = context.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
            
            var result = await friendshipService.GetFriendsAsync((Guid)userId);
            return result.ToApiResponse("Friends retrieved successfully");
        });

        group.MapGet("/requests/received", async (HttpContext context, IFriendshipService friendshipService) =>
        {
            var userId = context.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
                
            var result = await friendshipService.GetReceivedFriendRequestsAsync((Guid)userId);
            return result.ToApiResponse("Received friend requests retrieved successfully");
        });

        group.MapGet("/requests/sent", async (HttpContext context, IFriendshipService friendshipService) =>
        {
            var userId = context.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();

            var result = await friendshipService.GetSentFriendRequestsAsync((Guid)userId);
            return result.ToApiResponse("Sent friend requests retrieved successfully");
        });

        group.MapPost("/request/{receiverId:guid}", async (
            Guid receiverId,
            HttpContext context,
            IFriendshipService friendshipService
        ) =>
        {
            var senderId = context.GetCurrentUserId();
            if (senderId is null)
                return Results.Unauthorized();

            var result = await friendshipService.SendFriendRequestAsync((Guid)senderId, receiverId);
            return result.ToApiResponse("Friend request sent successfully");
        });

        group.MapPut("/accept/{senderId:guid}", async (
            Guid senderId,
            HttpContext context,
            IFriendshipService friendshipService
        ) =>
        {
            var receiverId = context.GetCurrentUserId();
            if (receiverId is null)
                return Results.Unauthorized();

            var result = await friendshipService.AcceptFriendRequestAsync(senderId, (Guid)receiverId);
            return result.ToApiResponse("Friend request accepted successfully");
        });

        group.MapDelete("/decline/{senderId:guid}", async (Guid senderId, HttpContext context, IFriendshipService friendshipService) =>
        {
            var receiverId = context.GetCurrentUserId();
            if (receiverId is null)
                return Results.Unauthorized();

            var result = await friendshipService.DeclineFriendRequestAsync(senderId, (Guid)receiverId);
            return result.ToApiResponse("Friend request declined successfully");
        });

        group.MapDelete("/{friendId:guid}", async (Guid friendId, HttpContext context, IFriendshipService friendshipService) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();
            var result = await friendshipService.RemoveFriendAsync((Guid)currentUserId, friendId);
            return result.ToApiResponse("Friend removed successfully");
        });

        group.MapGet("/status/{userId:guid}", async (
            Guid userId,
            HttpContext context,
            IFriendshipService friendshipService
        ) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();

            var result = await friendshipService.GetFriendshipStatusAsync((Guid)currentUserId, userId);
            return result.ToApiResponse("Friendship status retrieved successfully");
        });

        return group;
    }
}
