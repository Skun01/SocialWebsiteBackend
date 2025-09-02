using System;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Endpoints;

public static class FileEndpoints
{
    public static RouteGroupBuilder MapFileEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Files");

        group.MapPost("/images/upload", async (IFileService fileService, IFormFile file) =>
        {
            var result = await fileService.UploadImageAsync(file);
            return result.IsSuccess ? Results.Ok(new { url = result.Value }) : Results.BadRequest(result.Error);
        }).DisableAntiforgery();

        return group;
    }
}
