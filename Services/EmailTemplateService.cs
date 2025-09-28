using Microsoft.Extensions.Configuration;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IConfiguration _config;

    public EmailTemplateService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public string GetPasswordResetTemplate(string username, string resetUrl, int expiresMinutes)
    {
        var appName = _config["App:Name"] ?? "Social Website";
        var supportEmail = _config["App:SupportEmail"] ?? "support@socialwebsite.com";

        return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Reset Your Password</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 10px; margin: 15px 0; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{appName}</h1>
        </div>
        <div class='content'>
            <h2>Password Reset Request</h2>
            <p>Hello <strong>{username}</strong>,</p>
            <p>We received a request to reset your password. If you made this request, click the button below:</p>
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Reset Password</a>
            </p>
            <div class='warning'>
                <strong>⚠️ Important:</strong> This link will expire in <strong>{expiresMinutes} minutes</strong>.
            </div>
            <p>If you didn't request this, you can safely ignore this email.</p>
        </div>
        <div class='footer'>
            <p>This email was sent by {appName}</p>
            <p>Contact us at <a href='mailto:{supportEmail}'>{supportEmail}</a></p>
        </div>
    </div>
</body>
</html>";
    }

    public string GetEmailConfirmationTemplate(string username, string confirmationUrl)
    {
        var appName = _config["App:Name"] ?? "Social Website";
        var supportEmail = _config["App:SupportEmail"] ?? "support@socialwebsite.com";

        return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Confirm Your Email</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #28a745; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to {appName}!</h1>
        </div>
        <div class='content'>
            <h2>Confirm Your Email Address</h2>
            <p>Hello <strong>{username}</strong>,</p>
            <p>Thank you for joining {appName}! Please confirm your email address:</p>
            <p style='text-align: center;'>
                <a href='{confirmationUrl}' class='button'>Confirm Email</a>
            </p>
            <p>If you didn't create an account, you can ignore this email.</p>
        </div>
        <div class='footer'>
            <p>This email was sent by {appName}</p>
            <p>Contact us at <a href='mailto:{supportEmail}'>{supportEmail}</a></p>
        </div>
    </div>
</body>
</html>";
    }

    public string GetWelcomeTemplate(string username)
    {
        var appName = _config["App:Name"] ?? "Social Website";
        var supportEmail = _config["App:SupportEmail"] ?? "support@socialwebsite.com";

        return $"<h1>Welcome {username} to {appName}!</h1><p>Your account is ready. Contact us at {supportEmail} if you need help.</p>";
    }
}
