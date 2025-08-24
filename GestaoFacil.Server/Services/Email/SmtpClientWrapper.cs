using System.Net;
using System.Net.Mail;

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client;

    public SmtpClientWrapper(string host, int port, string user, string pass)
    {
        _client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };
    }

    public Task SendMailAsync(MailMessage message)
    {
        return _client.SendMailAsync(message);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}