using System;

namespace SocialWebsite.Shared;

public record class Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
