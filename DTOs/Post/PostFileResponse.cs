using Microsoft.Data.SqlClient;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.DTOs.Post;

public record class PostFileResponse(
    string Url,
    long FileSizeBytes,
    string MimeType,
    int SortOrder
);