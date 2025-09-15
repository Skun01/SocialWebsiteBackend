namespace SocialWebsite.DTOs.Post;

public record class PostFilesUploadRequest(
    List<IFormFile> Files
);