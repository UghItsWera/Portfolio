using System.Net;
using System.Net.Mail;

namespace PortfolioCMS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendContactEmailAsync(string fromName, string fromEmail, string message)
        {
            var host = _config["Email:SmtpHost"]!;
            var port = int.Parse(_config["Email:SmtpPort"]!);
            var user = _config["Email:SmtpUser"]!;
            var pass = _config["Email:SmtpPass"]!;
            var to   = _config["Email:ToAddress"]!;

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

            await client.SendMailAsync(mail);
        }
    }
}