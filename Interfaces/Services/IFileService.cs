using System;
using SocialWebsite.Entities;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IFileService
{
    void DeleteFile(string imageUrl);
    Task<FileAsset> UploadFileAsync(IFormFile file, List<string> validExtentions, string folderName);
}
