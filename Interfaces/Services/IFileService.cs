using System;
using SocialWebsite.Shared;

namespace SocialWebsite.Interfaces.Services;

public interface IFileService
{
    Task<Result<string>> UploadImageAsync(IFormFile file);
    void DeleteImage(string imageUrl);
}
