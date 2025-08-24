using System.Net.Mail;

public interface ISmtpClientWrapper : IDisposable
{
    Task SendMailAsync(MailMessage message);
}