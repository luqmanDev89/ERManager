using Microsoft.AspNetCore.Identity.UI.Services;

namespace ERManager.ViewModels
{
    public class NoOpEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This method is intentionally left empty to act as a No-Op
            return Task.CompletedTask;
        }
    }

}
