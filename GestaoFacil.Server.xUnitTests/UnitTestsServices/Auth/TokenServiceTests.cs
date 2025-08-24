using System;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Server.Services.Auth;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("MinhaChaveSecretaTesteMock123456");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("MinhaApi");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("MeuPublico");
        _configurationMock.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");
    }

    [Fact]
    public void Construtor_DeveCriarServicoComConfiguracaoValida()
    {
        var service = new TokenService(_configurationMock.Object);
        service.Should().NotBeNull();
    }

    [Fact]
    public void Construtor_DeveLancarQuandoConfiguracaoFaltando()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns((string?)null);

        Action act = () => new TokenService(configMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Jwt:Key*");
    }

    [Fact]
    public void Construtor_DeveLancarQuandoExpireMinutesInvalido()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("key");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");
        configMock.Setup(c => c["Jwt:ExpireMinutes"]).Returns("invalido");

        Action act = () => new TokenService(configMock.Object);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Jwt:ExpireMinutes*");
    }

    [Fact]
    public void GenerateToken_DeveRetornarTokenEExpiracao()
    {
        var service = new TokenService(_configurationMock.Object);
        var usuario = new UsuarioModel
        {
            Id = 1,
            Email = "teste@teste.com",
            TipoUsuario = new TipoUsuarioModel { Nome = "Admin" }
        };

        var (token, expiraEm) = service.GenerateToken(usuario);

        token.Should().NotBeNullOrEmpty();
        expiraEm.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateRefreshToken_DeveRetornarString64BytesBase64()
    {
        var service = new TokenService(_configurationMock.Object);

        var refreshToken = service.GenerateRefreshToken();

        refreshToken.Should().NotBeNullOrEmpty();
        var bytes = Convert.FromBase64String(refreshToken);
        bytes.Length.Should().Be(64);
    }

    [Fact]
    public void GeneratePasswordResetToken_DeveRetornarString32BytesBase64()
    {
        var service = new TokenService(_configurationMock.Object);

        var resetToken = service.GeneratePasswordResetToken();

        resetToken.Should().NotBeNullOrEmpty();
        var bytes = Convert.FromBase64String(resetToken);
        bytes.Length.Should().Be(32);
    }
}