namespace CreateRule.Services
{
    public interface IEmailService
    {
        Task<bool> SendRegistrationApprovedEmailAsync(string email, string username);
        Task<bool> SendRegistrationRejectedEmailAsync(string email, string reason);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
