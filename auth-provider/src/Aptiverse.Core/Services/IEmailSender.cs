namespace Aptiverse.Core.Services
{
    public interface IEmailSender
    {
        Task SendTemplateEmailAsync(string email, string subject, string templateType, object templateData);
    }
}
