using AutoMapper;
using FluentAssertions;
using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Services.Despesa;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestaoFacil.Server.xUnitTests.UnitTestsServices.Financeiro
{
    public class DespesaServiceTests
    {
        private readonly Mock<IDespesaRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DespesaService>> _loggerMock;
        private readonly DespesaService _service;

        public DespesaServiceTests()
        {
            _repositoryMock = new Mock<IDespesaRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DespesaService>>();

            _service = new DespesaService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDespesaQuandoEncontrada()
        {
            // Arrange
            var despesa = new DespesaModel { Id = 1, UsuarioId = 10, Nome = "Teste" };
            var dto = new DespesaDto { Id = 1, Nome = "Teste" };

            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(despesa);
            _mapperMock.Setup(m => m.Map<DespesaDto>(despesa)).Returns(dto);

            // Act
            var result = await _service.GetByIdAsync(1, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados.Should().BeEquivalentTo(dto);
            result.Mensagem.Should().Be("Despesa localizada com sucesso.");
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarFalhaQuandoNaoEncontrada()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99, 10)).ReturnsAsync((DespesaModel?)null);

            // Act
            var result = await _service.GetByIdAsync(99, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Despesa não encontrada.");
            result.Dados.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_DeveCriarDespesaComSucesso()
        {
            // Arrange
            var dtoCreate = new DespesaCreateDto { Nome = "Nova Despesa" };
            var despesa = new DespesaModel { Id = 1, Nome = "Nova Despesa", UsuarioId = 10 };
            var dtoResult = new DespesaDto { Id = 1, Nome = "Nova Despesa" };

            _mapperMock.Setup(m => m.Map<DespesaModel>(dtoCreate)).Returns(despesa);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<DespesaModel>())).ReturnsAsync(despesa);
            _mapperMock.Setup(m => m.Map<DespesaDto>(despesa)).Returns(dtoResult);

            // Act
            var result = await _service.CreateAsync(dtoCreate, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados.Should().BeEquivalentTo(dtoResult);
            result.Mensagem.Should().Be("Despesa criada com sucesso.");
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarFalhaQuandoIdsNaoBatem()
        {
            // Arrange
            var dtoUpdate = new DespesaUpdateDto { Id = 2, Nome = "Teste Update" };

            // Act
            var result = await _service.UpdateAsync(1, dtoUpdate, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("ID da despesa inválido.");
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverDespesaQuandoEncontrada()
        {
            // Arrange
            var despesa = new DespesaModel { Id = 1, UsuarioId = 10 };
            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(despesa);

            // Act
            var result = await _service.DeleteAsync(1, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados.Should().BeTrue();
            result.Mensagem.Should().Be("Despesa removida com sucesso.");
        }

        [Fact]
        public async Task ExportarExcelCompletoAsync_DeveRetornarFalhaQuandoSemDespesas()
        {
            // Arrange
            var filtro = new DespesaFiltroDto();
            _repositoryMock.Setup(r => r.FiltrarAsync(10, filtro)).ReturnsAsync(new List<DespesaModel>());

            // Act
            var result = await _service.ExportarExcelCompletoAsync(10, filtro);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Nenhuma despesa encontrada para exportação.");
        }

        [Fact]
        public async Task ExportarExcelCompletoAsync_DeveRetornarExcelValidoQuandoHaDespesas()
        {
            // Arrange
            var filtro = new DespesaFiltroDto();
            var despesas = new List<DespesaModel>
            {
                new DespesaModel { Id = 1, UsuarioId = 10, Nome = "Luz", Data = DateTime.Now, Valor = 100 }
            };

            _repositoryMock.Setup(r => r.FiltrarAsync(10, filtro)).ReturnsAsync(despesas);

            // Act
            var result = await _service.ExportarExcelCompletoAsync(10, filtro);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados.Should().NotBeNull();
            result.Dados.Length.Should().BeGreaterThan(0); // Excel gerado
        }
    }
}
