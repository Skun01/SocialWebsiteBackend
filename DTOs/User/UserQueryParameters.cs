namespace SocialWebsite.DTOs.User;

public record class UserQueryParameters
{
    private const int MaxPageSize = 30;
    private int _pageSize = 10;
    public string? Name { set; get; }
    public string? SortBy { set; get; } = "name";
    public int PageNumber { set; get; }
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
