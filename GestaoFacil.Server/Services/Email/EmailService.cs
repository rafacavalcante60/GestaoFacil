using System.Net.Mail;

namespace GestaoFacil.Server.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly Func<ISmtpClientWrapper> _smtpClientFactory;

        public EmailService(IConfiguration config, Func<ISmtpClientWrapper> smtpClientFactory)
        {
            _config = config;
            _smtpClientFactory = smtpClientFactory;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPortStr = _config["Email:SmtpPort"];
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var from = _config["Email:From"];

            if (string.IsNullOrWhiteSpace(smtpHost))
            {
                throw new ArgumentNullException(nameof(smtpHost), "SMTP host não configurado.");
            }
            if (string.IsNullOrWhiteSpace(smtpPortStr))
            {
                throw new ArgumentNullException(nameof(smtpPortStr), "SMTP port não configurado.");
            }
            if (string.IsNullOrWhiteSpace(smtpUser))
            {
                throw new ArgumentNullException(nameof(smtpUser), "SMTP user não configurado.");
            }
            if (string.IsNullOrWhiteSpace(smtpPass))
            {
                throw new ArgumentNullException(nameof(smtpPass), "SMTP pass não configurado.");
            }
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException(nameof(from), "Email de origem não configurado.");
            }


            var smtpPort = int.Parse(smtpPortStr);

            using var client = _smtpClientFactory();
            var mailMessage = new MailMessage(from, to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
