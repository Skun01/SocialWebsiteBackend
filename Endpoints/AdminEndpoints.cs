using Microsoft.AspNetCore.Mvc;
using SocialWebsite.Extensions;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.DTOs.User;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Admin")
            .RequireAuthorization()
            .RequireAdmin();

        group.MapPut("/users/{userId:guid}/role", async (
            Guid userId,
            [FromBody] SetUserRoleRequest request,
            IUserService userService
        ) =>
        {
            var result = await userService.SetUserRoleAsync(userId, request.Role);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        return group;
    }
}
