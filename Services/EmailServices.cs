using System.Net;
using System.Net.Mail;

namespace PortfolioCMS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendContactEmailAsync(string fromName, string fromEmail, string message)
        {
            var host = _config["Email:SmtpHost"]!;
            var port = int.Parse(_config["Email:SmtpPort"]!);
            var user = _config["Email:SmtpUser"]!;
            var pass = _config["Email:SmtpPass"]!;
            var to   = _config["Email:ToAddress"]!;

            _logger.LogInformation("Attempting to send email from {FromEmail} to {To} via {Host}:{Port}", 
                fromEmail, to, host, port);

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(user, "Portfolio Contact Form"),
                Subject = $"New message from {fromName}",
                Body = $"Name: {fromName}\nEmail: {fromEmail}\n\n{message}",
                IsBodyHtml = false
            };

            mail.To.Add(to);
            mail.ReplyToList.Add(new MailAddress(fromEmail, fromName));

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }
    }
}