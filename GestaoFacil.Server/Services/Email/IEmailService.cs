namespace GestaoFacil.Server.Services.Email
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
