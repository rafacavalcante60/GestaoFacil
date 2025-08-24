using AutoMapper;
using FluentAssertions;
using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Auth;
using GestaoFacil.Server.Models.Auth;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Server.Services.Auth;
using GestaoFacil.Server.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly AppDbContext _context;

    public AuthServiceTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _mapperMock = new Mock<IMapper>();
        _emailServiceMock = new Mock<IEmailService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    private AuthService CreateService()
    {
        return new AuthService(
            _usuarioRepoMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _context,
            _emailServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_DeveFalhar_QuandoEmailInvalido()
    {
        var service = CreateService();
        var dto = new UsuarioLoginDto { Email = "emailinvalido", Senha = "123" };

        var result = await service.LoginAsync(dto);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Email inválido.");
    }

    [Fact]
    public async Task LoginAsync_DeveFalhar_QuandoCredenciaisInvalidas()
    {
        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("teste@teste.com"))
            .ReturnsAsync((UsuarioModel?)null);

        var service = CreateService();
        var dto = new UsuarioLoginDto { Email = "teste@teste.com", Senha = "123" };

        var result = await service.LoginAsync(dto);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Email ou senha inválidos.");
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var usuario = new UsuarioModel
        {
            Id = 1,
            Email = "teste@teste.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123"),
            TipoUsuario = new TipoUsuarioModel { Nome = "Admin" }
        };

        _usuarioRepoMock.Setup(r => r.GetByEmailAsync(usuario.Email)).ReturnsAsync(usuario);
        _tokenServiceMock.Setup(t => t.GenerateToken(usuario)).Returns(("token123", DateTime.UtcNow.AddMinutes(30)));
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh123");

        var service = CreateService();
        var dto = new UsuarioLoginDto { Email = usuario.Email, Senha = "123" };

        var result = await service.LoginAsync(dto);

        result.Status.Should().BeTrue();
        result.Dados.Token.Should().Be("token123");
        result.Dados.RefreshToken.Should().Be("refresh123");
    }

    [Fact]
    public async Task RegisterAsync_DeveFalhar_QuandoEmailJaExiste()
    {
        _usuarioRepoMock.Setup(r => r.EmailExistsAsync("teste@teste.com")).ReturnsAsync(true);

        var service = CreateService();
        var dto = new UsuarioRegisterDto { Email = "teste@teste.com", Senha = "123" };

        var result = await service.RegisterAsync(dto);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Email já está em uso.");
    }

    [Fact]
    public async Task RegisterAsync_DeveRegistrarUsuarioComSucesso()
    {
        var dto = new UsuarioRegisterDto { Email = "novo@teste.com", Senha = "123" };
        var tipoUsuario = new TipoUsuarioModel { Id = 1, Nome = "Comum" };

        _usuarioRepoMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _usuarioRepoMock.Setup(r => r.GetTipoUsuarioByNameAsync("Comum")).ReturnsAsync(tipoUsuario);
        _mapperMock.Setup(m => m.Map<UsuarioModel>(dto)).Returns(new UsuarioModel { Email = dto.Email });

        _usuarioRepoMock.Setup(r => r.AddAsync(It.IsAny<UsuarioModel>()))
            .ReturnsAsync((UsuarioModel u) => u);

        var service = CreateService();
        var result = await service.RegisterAsync(dto);

        result.Status.Should().BeTrue();
        result.Mensagem.Should().Be("Operação realizada com sucesso.");
    }

    [Fact]
    public async Task LogoutAsync_DeveFalhar_QuandoTokenNaoExiste()
    {
        var service = CreateService();
        var result = await service.LogoutAsync("naoexiste");

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Token não encontrado.");
    }

    [Fact]
    public async Task LogoutAsync_DeveRevogarToken()
    {
        var token = new RefreshTokenModel
        {
            Token = "token123",
            UsuarioId = 1,
            ExpiraEm = DateTime.UtcNow.AddDays(1),
            EstaRevogado = false
        };
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();

        var service = CreateService();
        var result = await service.LogoutAsync("token123");

        result.Status.Should().BeTrue();
        result.Mensagem.Should().Be("Operação realizada com sucesso.");
        token.EstaRevogado.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshTokenAsync_DeveFalhar_QuandoTokenInvalido()
    {
        var service = CreateService();
        var result = await service.RefreshTokenAsync("invalido");

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Refresh token inválido ou expirado.");
    }

    [Fact]
    public async Task RefreshTokenAsync_DeveGerarNovoToken()
    {
        var usuario = new UsuarioModel
        {
            Id = 1,
            Email = "teste@teste.com",
            TipoUsuario = new TipoUsuarioModel { Nome = "Admin" }
        };
        var tokenModel = new RefreshTokenModel
        {
            Token = "validotoken",
            UsuarioId = usuario.Id,
            Usuario = usuario,
            ExpiraEm = DateTime.UtcNow.AddDays(1),
            EstaRevogado = false
        };
        _context.RefreshTokens.Add(tokenModel);
        await _context.SaveChangesAsync();

        _tokenServiceMock.Setup(t => t.GenerateToken(usuario)).Returns(("novoToken", DateTime.UtcNow.AddMinutes(30)));
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refreshNovo");

        var service = CreateService();
        var result = await service.RefreshTokenAsync("validotoken");

        result.Status.Should().BeTrue();
        result.Dados.Token.Should().Be("novoToken");
        result.Dados.RefreshToken.Should().Be("refreshNovo");
    }

    [Fact]
    public async Task ForgotPasswordAsync_DeveRetornarSucessoMesmoSeEmailNaoExiste()
    {
        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("naoexiste@teste.com"))
            .ReturnsAsync((UsuarioModel?)null);

        var service = CreateService();
        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "naoexiste@teste.com" });

        result.Status.Should().BeTrue();
        result.Mensagem.Should().Be("Operação realizada com sucesso.");
    }

    [Fact]
    public async Task ForgotPasswordAsync_DeveFalhar_QuandoEmailServiceErro()
    {
        var usuario = new UsuarioModel { Id = 1, Email = "teste@teste.com" };
        _usuarioRepoMock.Setup(r => r.GetByEmailAsync(usuario.Email)).ReturnsAsync(usuario);
        _tokenServiceMock.Setup(t => t.GeneratePasswordResetToken()).Returns("tokenReset");
        _usuarioRepoMock.Setup(r => r.UpdateAsync(usuario)).Returns(Task.CompletedTask);

        _emailServiceMock.Setup(e => e.SendAsync(usuario.Email, It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Falha envio"));

        var service = CreateService();
        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = usuario.Email });

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Não foi possível enviar o email de redefinição. Tente novamente mais tarde.");
    }

    [Fact]
    public async Task ResetPasswordAsync_DeveFalhar_QuandoTokenInvalido()
    {
        var service = CreateService();
        var result = await service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = "invalido", NewPassword = "nova" });

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Token inválido ou expirado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_DeveAtualizarSenha_QuandoTokenValido()
    {
        var usuario = new UsuarioModel
        {
            Id = 1,
            Email = "teste@teste.com",
            PasswordResetToken = "tokenvalido",
            PasswordResetTokenExpiraEm = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        _usuarioRepoMock.Setup(r => r.UpdateAsync(usuario)).Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = "tokenvalido", NewPassword = "novaSenha" });

        result.Status.Should().BeTrue();
        result.Mensagem.Should().Be("Operação realizada com sucesso.");
        usuario.PasswordResetToken.Should().BeNull();
        usuario.PasswordResetTokenExpiraEm.Should().BeNull();
    }
}
