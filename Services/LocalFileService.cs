using System;
using Serilog;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class LocalFileService(IWebHostEnvironment webHostEnvironment) : IFileService
{

    public void DeleteFile(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;
        var contentPath = webHostEnvironment.ContentRootPath;
        var filePath = Path.Combine(contentPath, "wwwroot", imageUrl.TrimStart('/'));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, List<string> validExtentions, string folderName)
    {
        var fileExtention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!validExtentions.Contains(fileExtention))
            throw new ArgumentException("File type is not valid in ({string.Join(',', validExtentions)})");

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
        string imagePath = $"/{folderName}/{newFileName}";
        return imagePath;
    }
}
