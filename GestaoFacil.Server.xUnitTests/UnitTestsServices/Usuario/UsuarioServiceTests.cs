using AutoMapper;
using FluentAssertions;
using GestaoFacil.Server.DTOs.Usuario;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Usuario;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UsuarioService>> _loggerMock;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UsuarioService>>();
        _service = new UsuarioService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuario_QuandoEncontrado()
    {
        var usuario = new UsuarioModel { Id = 1, Nome = "Teste", Email = "teste@teste.com" };
        var dto = new UsuarioDto { Id = 1, Nome = "Teste", Email = "teste@teste.com" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _mapperMock.Setup(m => m.Map<UsuarioDto>(usuario)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        result.Status.Should().BeTrue();
        result.Dados.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarFalha_QuandoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UsuarioModel?)null);

        var result = await _service.GetByIdAsync(1);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Usuário não encontrado.");
        result.Dados.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_DeveRetornarUsuariosPaginados()
    {
        var usuarios = new PagedList<UsuarioModel>(new List<UsuarioModel>
        {
            new UsuarioModel { Id = 1, Nome = "Teste", Email = "teste@teste.com" }
        }, 1, 1, 10);

        var dtos = new List<UsuarioDto>
        {
            new UsuarioDto { Id = 1, Nome = "Teste", Email = "teste@teste.com" }
        };

        _repositoryMock.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync(usuarios);
        _mapperMock.Setup(m => m.Map<List<UsuarioDto>>(usuarios)).Returns(dtos);

        var parameters = new QueryStringParameters { PageNumber = 1, PageSize = 10 };
        var result = await _service.GetPagedAsync(parameters);

        result.Status.Should().BeTrue();
        result.Dados.Should().NotBeNull();
        result.Dados.TotalCount.Should().Be(1);
        result.Mensagem.Should().Be("Usuários paginados carregados com sucesso.");
    }

    [Fact]
    public async Task UpdatePerfilAsync_DeveRetornarFalha_QuandoUsuarioNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UsuarioModel?)null);

        var dto = new UsuarioUpdateDto { Nome = "NovoNome", Email = "novo@teste.com" };
        var result = await _service.UpdatePerfilAsync(1, dto);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task UpdatePerfilAsync_DeveAtualizar_QuandoUsuarioExiste()
    {
        var usuario = new UsuarioModel { Id = 1, Nome = "Teste", Email = "teste@teste.com" };
        var dto = new UsuarioUpdateDto { Nome = "NovoNome", Email = "novo@teste.com" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _mapperMock.Setup(m => m.Map(dto, usuario));
        _repositoryMock.Setup(r => r.UpdateAsync(usuario)).Returns(Task.CompletedTask);

        var result = await _service.UpdatePerfilAsync(1, dto);

        result.Status.Should().BeTrue();
        result.Dados.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAdminAsync_DeveRetornarFalha_QuandoUsuarioNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UsuarioModel?)null);

        var dto = new UsuarioAdminUpdateDto { Id = 1, Nome = "Admin", Email = "admin@teste.com", TipoUsuarioId = 1 };
        var result = await _service.UpdateAdminAsync(1, dto);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task UpdateAdminAsync_DeveAtualizar_QuandoUsuarioExiste()
    {
        var usuario = new UsuarioModel { Id = 1, Nome = "Teste", Email = "teste@teste.com" };
        var dto = new UsuarioAdminUpdateDto { Id = 1, Nome = "Admin", Email = "admin@teste.com", TipoUsuarioId = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _mapperMock.Setup(m => m.Map(dto, usuario));
        _repositoryMock.Setup(r => r.UpdateAsync(usuario)).Returns(Task.CompletedTask);

        var result = await _service.UpdateAdminAsync(1, dto);

        result.Status.Should().BeTrue();
        result.Dados.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAdminAsync_DeveRetornarFalha_QuandoUsuarioNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UsuarioModel?)null);

        var result = await _service.DeleteAdminAsync(1);

        result.Status.Should().BeFalse();
        result.Mensagem.Should().Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task DeleteAdminAsync_DeveDeletar_QuandoUsuarioExiste()
    {
        var usuario = new UsuarioModel { Id = 1, Nome = "Teste", Email = "teste@teste.com" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _repositoryMock.Setup(r => r.DeleteAsync(usuario)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAdminAsync(1);

        result.Status.Should().BeTrue();
        result.Dados.Should().BeTrue();
    }
}