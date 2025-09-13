using System;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IFileService
{
    void DeleteFile(string imageUrl);
    Task<string> UploadFileAsync(IFormFile file, List<string> validExtentions, string folderName);
}
