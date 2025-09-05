using System;

namespace SocialWebsite.Interfaces.Services;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
