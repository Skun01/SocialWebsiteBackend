using System.Threading.Tasks;

namespace SocialWebsite.Interfaces.Services;

public interface IEmailTemplateService
{
    string GetPasswordResetTemplate(string username, string resetUrl, int expiresMinutes);
    string GetEmailConfirmationTemplate(string username, string confirmationUrl);
    string GetWelcomeTemplate(string username);
}
