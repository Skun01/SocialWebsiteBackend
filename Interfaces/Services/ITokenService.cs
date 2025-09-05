using System;
using SocialWebsite.Entities;

namespace SocialWebsite.Interfaces.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateEmailConfirmationToken(User user);
    Guid? ValidateAndGetUserIdFromEmailToken(string token);
}
