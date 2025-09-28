using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _secretKey;
    public TokenService(IConfiguration configuration)
    {
        _config = configuration;
        _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    }
    public string CreateAccessToken(User user)
    {
        var credenticals = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserName", user.Username),
            new Claim("role", user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpirationHours"]!)),
            signingCredentials: credenticals
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public string CreateEmailConfirmationToken(User user)
    {
        var credenticals = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature);
        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("purpose", "email_confirmation")
        };
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpirationHours"]!)),
            claims: claims,
            signingCredentials: credenticals
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public Guid? ValidateAndGetUserIdFromEmailToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = _secretKey,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var purposeClaim = principal.FindFirst("purpose");
            if (purposeClaim is null || purposeClaim.Value != "email_confirmation")
                return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            return userId;
        }
        catch (Exception)
        {
            return null;
        }
        
    }
}
