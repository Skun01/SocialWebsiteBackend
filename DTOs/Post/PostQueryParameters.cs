namespace SocialWebsite.DTOs.Post;

public record class PostQueryParameters
{
    public int PageSize { get; set; } = 10;
    public string? Cursor { get; set; } 
}
