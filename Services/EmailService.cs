using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CreateRule.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendRegistrationApprovedEmailAsync(string email, string username)
        {
            var loginUrl = $"{_configuration["ApplicationUrl"] ?? "http://localhost:5198"}/Account/Login";
            
            var subject = "注册审核通过 - CreateRule 系统";
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #28a745;'>注册审核通过通知</h2>
        <p>尊敬的 <strong>{username}</strong>，</p>
        <p>恭喜！您的注册申请已通过审核，现在可以登录系统了。</p>
        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>登录地址：</strong></p>
            <p><a href='{loginUrl}' style='color: #007bff;'>{loginUrl}</a></p>
        </div>
        <p>建议您首次登录后立即修改密码，以保障账户安全。</p>
        <p>如有任何问题，请联系系统管理员。</p>
        <hr style='border: 1px solid #ddd; margin: 30px 0;'>
        <p style='color: #666; font-size: 12px;'>此邮件由系统自动发送，请勿回复。</p>
    </div>
</body>
</html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendRegistrationRejectedEmailAsync(string email, string reason)
        {
            var subject = "注册审核结果通知 - CreateRule 系统";
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #dc3545;'>注册审核结果通知</h2>
        <p>您好，</p>
        <p>很抱歉地通知您，您的注册申请未能通过审核。</p>
        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>拒绝原因：</strong></p>
            <p>{reason}</p>
        </div>
        <p>如有疑问，请联系系统管理员。</p>
        <hr style='border: 1px solid #ddd; margin: 30px 0;'>
        <p style='color: #666; font-size: 12px;'>此邮件由系统自动发送，请勿回复。</p>
    </div>
</body>
</html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"邮件发送成功: {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"邮件发送失败: {to}");
                return false;
            }
        }
    }
}
