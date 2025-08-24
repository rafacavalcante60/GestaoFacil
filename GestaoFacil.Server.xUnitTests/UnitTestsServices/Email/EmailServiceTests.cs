using FluentAssertions;
using GestaoFacil.Server.Services.Email;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

public class EmailServiceTests
{
    [Fact]
    public async Task SendAsync_DeveChamarSendMailAsync_DoWrapper()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Email:SmtpHost"]).Returns("smtp.test.com");
        configMock.Setup(c => c["Email:SmtpPort"]).Returns("587");
        configMock.Setup(c => c["Email:SmtpUser"]).Returns("user@test.com");
        configMock.Setup(c => c["Email:SmtpPass"]).Returns("password");
        configMock.Setup(c => c["Email:From"]).Returns("from@test.com");

        var smtpClientMock = new Mock<ISmtpClientWrapper>();
        smtpClientMock.Setup(c => c.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);

        // Factory retorna o mock
        Func<ISmtpClientWrapper> factory = () => smtpClientMock.Object;

        var service = new EmailService(configMock.Object, factory);

        var to = "destino@test.com";
        var subject = "Assunto";
        var body = "<b>Mensagem</b>";

        // Act
        await service.SendAsync(to, subject, body);

        // Assert
        smtpClientMock.Verify(c => c.SendMailAsync(It.Is<MailMessage>(
            m => m.From.Address == "from@test.com" &&
                 m.To[0].Address == to &&
                 m.Subject == subject &&
                 m.Body == body &&
                 m.IsBodyHtml)), Times.Once);
    }

    [Fact]
    public async Task SendAsync_DeveLancarExcecao_QuandoConfigInvalida()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Email:SmtpHost"]).Returns((string?)null);
        configMock.Setup(c => c["Email:SmtpPort"]).Returns("587");
        configMock.Setup(c => c["Email:SmtpUser"]).Returns("user@test.com");
        configMock.Setup(c => c["Email:SmtpPass"]).Returns("password");
        configMock.Setup(c => c["Email:From"]).Returns("from@test.com");

        var smtpClientMock = new Mock<ISmtpClientWrapper>();
        Func<ISmtpClientWrapper> factory = () => smtpClientMock.Object;

        var service = new EmailService(configMock.Object, factory);

        var to = "destino@test.com";
        var subject = "Assunto";
        var body = "<b>Mensagem</b>";

        // Act & Assert
        await Assert.ThrowsAsync<System.ArgumentNullException>(() => service.SendAsync(to, subject, body));
    }

    [Fact]
    public async Task SendAsync_DevePropagarExcecao_DoWrapper()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Email:SmtpHost"]).Returns("smtp.test.com");
        configMock.Setup(c => c["Email:SmtpPort"]).Returns("587");
        configMock.Setup(c => c["Email:SmtpUser"]).Returns("user@test.com");
        configMock.Setup(c => c["Email:SmtpPass"]).Returns("password");
        configMock.Setup(c => c["Email:From"]).Returns("from@test.com");

        var smtpClientMock = new Mock<ISmtpClientWrapper>();
        smtpClientMock.Setup(c => c.SendMailAsync(It.IsAny<MailMessage>()))
            .ThrowsAsync(new SmtpException("Falha ao enviar"));

        Func<ISmtpClientWrapper> factory = () => smtpClientMock.Object;

        var service = new EmailService(configMock.Object, factory);

        var to = "destino@test.com";
        var subject = "Assunto";
        var body = "<b>Mensagem</b>";

        // Act & Assert
        await Assert.ThrowsAsync<SmtpException>(() => service.SendAsync(to, subject, body));
    }
}   