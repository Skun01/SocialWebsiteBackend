using System;
using Serilog;
using SocialWebsite.Entities;
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

    public async Task<FileAsset> UploadFileAsync(
        IFormFile file,
        List<string> validExtensions,
        string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!validExtensions.Contains(extension))
            throw new ArgumentException(
                $"File type is not valid. Allowed: {string.Join(',', validExtensions)}");

        // root folder
        var contentPath = webHostEnvironment.ContentRootPath;
        var uploadPath = Path.Combine(contentPath, "wwwroot", folderName);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // create new file name
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadPath, newFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileAsset = new FileAsset
        {
            Id = Guid.NewGuid(),
            StorageKey = Path.Combine(folderName, newFileName).Replace("\\", "/"),
            MimeType = file.ContentType,
            FileSizeBytes = file.Length,
            OriginalFileName = file.FileName,
            CreatedAt = DateTime.UtcNow
        };

        return fileAsset;
    }
}
