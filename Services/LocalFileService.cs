using System;
using Serilog;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class LocalFileService(IWebHostEnvironment webHostEnvironment) : IFileService
{

    public void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;
        var contentPath = webHostEnvironment.ContentRootPath;
        var filePath = Path.Combine(contentPath, "wwwroot", imageUrl.TrimStart('/'));

        if (File.Exists(filePath))
        {
        File.Delete(filePath);
        }
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return Result.Failure<string>(new Error("400", "No file to upload"));

        List<string> validExtentions = [".jpg", ".png"];
        var fileExtention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!validExtentions.Contains(fileExtention))
            return Result.Failure<string>(new Error("404", $"File type is not valid in ({string.Join(',', validExtentions)})"));

        var contentPath = webHostEnvironment.ContentRootPath;
        var path = Path.Combine(contentPath, "wwwroot/images");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var newFileName = $"{Guid.NewGuid()}{fileExtention}";
        var filePath = Path.Combine(path, newFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        string imagePath = $"/images/{newFileName}";
        return Result.Success(imagePath);
    }
}
