using System;
using System.Net;
using System.Net.Mail;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly string _smtpServer;
    private readonly int _port;
    private readonly string _fromAddress;
    private readonly string _password;
    public EmailSenderService(IConfiguration configuration)
    {
        _smtpServer = configuration["EmailSettings:SmtpServer"]!;
        _port = int.Parse(configuration["EmailSettings:Port"])!;
        _fromAddress = configuration["EmailSettings:FromAddress"]!;
        _password = configuration["EmailSettings:Password"]!;
    }
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_smtpServer, _port)
        {
            Credentials = new NetworkCredential(_fromAddress, _password),
            EnableSsl = true 
        };
        return client.SendMailAsync(
            new MailMessage(_fromAddress, email, subject, htmlMessage) { IsBodyHtml = true }
        );
    }
}
