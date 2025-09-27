using System;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;

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
            return Results.Ok(result.Value);
        });

        group.MapGet("/requests/received", async (HttpContext context, IFriendshipService friendshipService) =>
        {
            var userId = context.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
                
            var result = await friendshipService.GetReceivedFriendRequestsAsync((Guid)userId);
            return Results.Ok(result.Value);
        });

        group.MapGet("/requests/sent", async (HttpContext context, IFriendshipService friendshipService) =>
        {
            var userId = context.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();

            var result = await friendshipService.GetSentFriendRequestsAsync((Guid)userId);
            return Results.Ok(result.Value);
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
            return result.IsSuccess
                ? Results.Ok("Friend request sent.") 
                : Results.BadRequest(result.Error);
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
            return result.IsSuccess
                ? Results.Ok("Friend request accepted.") 
                : Results.NotFound(result.Error);
        });

        group.MapDelete("/decline/{senderId:guid}", async (Guid senderId, HttpContext context, IFriendshipService friendshipService) =>
        {
            var receiverId = context.GetCurrentUserId();
            if (receiverId is null)
                return Results.Unauthorized();

            var result = await friendshipService.DeclineFriendRequestAsync(senderId, (Guid)receiverId);
            return result.IsSuccess
                ? Results.NoContent() 
                : Results.NotFound(result.Error);
        });

        group.MapDelete("/{friendId:guid}", async (Guid friendId, HttpContext context, IFriendshipService friendshipService) =>
        {
            var currentUserId = context.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();
            var result = await friendshipService.RemoveFriendAsync((Guid)currentUserId, friendId);
            return result.IsSuccess
                ? Results.NoContent() 
                : Results.NotFound(result.Error);
        });

        return group;
    }
}
