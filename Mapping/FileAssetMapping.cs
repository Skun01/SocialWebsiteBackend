using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;

namespace SocialWebsite.Mapping;

public static class FileAssetMapping
{
    public static PostFileResponse ToResponse(this PostFile postFile, string baseUrl)
    {
        return new(
            Url: baseUrl + postFile.FileAsset.StorageKey,
            FileSizeBytes: postFile.FileAsset.FileSizeBytes,
            MimeType: postFile.FileAsset.MimeType,
            SortOrder: postFile.SortOrder
        );
    }
}
