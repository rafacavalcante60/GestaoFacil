using AutoMapper;
using FluentAssertions;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Services.Cache;
using GestaoFacil.Server.Services.Financeiro;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestaoFacil.Server.xUnitTests.UnitTestsServices.Financeiro
{
    public class ReceitaServiceTests
    {
        private readonly Mock<IReceitaRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReceitaService>> _loggerMock;
        private readonly Mock<IRelatorioCache> _cacheMock;
        private readonly ReceitaService _service;

        public ReceitaServiceTests()
        {
            _repositoryMock = new Mock<IReceitaRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReceitaService>>();
            _cacheMock = new Mock<IRelatorioCache>();

            _service = new ReceitaService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarReceitaQuandoEncontrada()
        {
            var receita = new ReceitaModel { Id = 1, UsuarioId = 10, Nome = "Salario" };
            var dto = new ReceitaDto { Id = 1, Nome = "Salario" };

            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(receita);
            _mapperMock.Setup(m => m.Map<ReceitaDto>(receita)).Returns(dto);

            var result = await _service.GetByIdAsync(1, 10);

            result.Status.Should().BeTrue();
            result.Dados.Should().BeEquivalentTo(dto);
            result.Mensagem.Should().Be("Receita localizada com sucesso.");
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarFalhaQuandoNaoEncontrada()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(99, 10)).ReturnsAsync((ReceitaModel?)null);

            var result = await _service.GetByIdAsync(99, 10);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Receita não encontrada.");
            result.Dados.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_DeveCriarReceitaComSucesso()
        {
            var dtoCreate = new ReceitaCreateDto { Nome = "Nova Receita" };
            var receita = new ReceitaModel { Id = 1, Nome = "Nova Receita", UsuarioId = 10 };
            var dtoResult = new ReceitaDto { Id = 1, Nome = "Nova Receita" };

            _mapperMock.Setup(m => m.Map<ReceitaModel>(dtoCreate)).Returns(receita);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ReceitaModel>())).ReturnsAsync(receita);
            _repositoryMock.Setup(r => r.CategoriaAcessivelAsync(It.IsAny<int>(), 10)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<ReceitaDto>(receita)).Returns(dtoResult);

            var result = await _service.CreateAsync(dtoCreate, 10);

            result.Status.Should().BeTrue();
            result.Dados.Should().BeEquivalentTo(dtoResult);
            result.Mensagem.Should().Be("Receita criada com sucesso.");
        }

        [Fact]
        public async Task CreateAsync_DeveFalharQuandoCategoriaNaoPertenceAoUsuario()
        {
            // Arrange
            var dtoCreate = new ReceitaCreateDto { Nome = "Nova Receita" };
            var receita = new ReceitaModel { Id = 1, Nome = "Nova Receita", UsuarioId = 10 };

            _mapperMock.Setup(m => m.Map<ReceitaModel>(dtoCreate)).Returns(receita);
            _repositoryMock.Setup(r => r.CategoriaAcessivelAsync(It.IsAny<int>(), 10)).ReturnsAsync(false);

            // Act
            var result = await _service.CreateAsync(dtoCreate, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Categoria inválida.");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ReceitaModel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarFalhaQuandoIdsNaoBatem()
        {
            var dtoUpdate = new ReceitaUpdateDto { Id = 2, Nome = "Teste Update" };

            var result = await _service.UpdateAsync(1, dtoUpdate, 10);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("ID da receita inválido.");
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverReceitaQuandoEncontrada()
        {
            var receita = new ReceitaModel { Id = 1, UsuarioId = 10 };
            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(receita);

            var result = await _service.DeleteAsync(1, 10);

            result.Status.Should().BeTrue();
            result.Dados.Should().BeTrue();
            result.Mensagem.Should().Be("Receita removida com sucesso.");
        }

        [Fact]
        public async Task ExportarExcelCompletoAsync_DeveRetornarFalhaQuandoSemReceitas()
        {
            var filtro = new ReceitaFiltroDto();
            _repositoryMock.Setup(r => r.FiltrarAsync(10, filtro)).ReturnsAsync(new List<ReceitaModel>());

            var result = await _service.ExportarExcelCompletoAsync(10, filtro);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Nenhuma receita encontrada para exportação.");
        }

        [Fact]
        public async Task ExportarExcelCompletoAsync_DeveRetornarExcelValidoQuandoHaReceitas()
        {
            var filtro = new ReceitaFiltroDto();
            var receitas = new List<ReceitaModel>
            {
                new ReceitaModel { Id = 1, UsuarioId = 10, Nome = "Salario", Data = DateTime.Now, Valor = 5000 }
            };

            _repositoryMock.Setup(r => r.FiltrarAsync(10, filtro)).ReturnsAsync(receitas);

            var result = await _service.ExportarExcelCompletoAsync(10, filtro);

            result.Status.Should().BeTrue();
            result.Dados.Should().NotBeNull();
            result.Dados.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_DeveInvalidarCacheDeRelatorios()
        {
            var dtoCreate = new ReceitaCreateDto { Nome = "Nova Receita" };
            var receita = new ReceitaModel { Id = 1, Nome = "Nova Receita", UsuarioId = 10 };

            _mapperMock.Setup(m => m.Map<ReceitaModel>(dtoCreate)).Returns(receita);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ReceitaModel>())).ReturnsAsync(receita);
            _repositoryMock.Setup(r => r.CategoriaAcessivelAsync(It.IsAny<int>(), 10)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<ReceitaDto>(receita)).Returns(new ReceitaDto());

            await _service.CreateAsync(dtoCreate, 10);

            // gravar uma receita muda os relatorios: o cache do usuario tem que ser invalidado
            _cacheMock.Verify(c => c.InvalidarAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
