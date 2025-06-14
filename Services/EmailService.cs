using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AgenciaDeViajes.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            
            if (smtpSettings == null || 
                string.IsNullOrEmpty(smtpSettings["Host"]) ||
                string.IsNullOrEmpty(smtpSettings["Port"]) ||
                string.IsNullOrEmpty(smtpSettings["Username"]) ||
                string.IsNullOrEmpty(smtpSettings["Password"]) ||
                string.IsNullOrEmpty(smtpSettings["FromAddress"]) ||
                string.IsNullOrEmpty(smtpSettings["FromName"]))
            {
                throw new InvalidOperationException("Configuración SMTP incompleta");
            }

            var client = new SmtpClient(smtpSettings["Host"]!)
            {
                Port = int.Parse(smtpSettings["Port"]!),
                Credentials = new NetworkCredential(
                    smtpSettings["Username"]!,
                    smtpSettings["Password"]!),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromAddress"]!, smtpSettings["FromName"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}