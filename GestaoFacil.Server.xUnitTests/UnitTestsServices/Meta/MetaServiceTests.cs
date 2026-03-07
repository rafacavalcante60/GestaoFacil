using AutoMapper;
using FluentAssertions;
using GestaoFacil.Server.DTOs.Meta;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Meta;
using GestaoFacil.Server.Services.Meta;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestaoFacil.Server.xUnitTests.UnitTestsServices.Meta
{
    public class MetaServiceTests
    {
        private readonly Mock<IMetaRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<MetaService>> _loggerMock;
        private readonly MetaService _service;

        public MetaServiceTests()
        {
            _repositoryMock = new Mock<IMetaRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<MetaService>>();

            _service = new MetaService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarMetaComDadosValidos()
        {
            // Arrange
            var dto = new MetaCreateDto
            {
                Nome = "Limite alimentação",
                ValorMeta = 500,
                Tipo = TipoMeta.Despesa,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            var meta = new MetaFinanceiraModel { Id = 1, UsuarioId = 10, Nome = "Limite alimentação", ValorMeta = 500 };
            var dtoResult = new MetaDto { Id = 1, Nome = "Limite alimentação", ValorMeta = 500 };

            _mapperMock.Setup(m => m.Map<MetaFinanceiraModel>(dto)).Returns(meta);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<MetaFinanceiraModel>())).ReturnsAsync(meta);
            _mapperMock.Setup(m => m.Map<MetaDto>(meta)).Returns(dtoResult);
            _repositoryMock.Setup(r => r.GetSomaDespesasAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int?>())).ReturnsAsync(0m);

            // Act
            var result = await _service.CreateAsync(dto, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Mensagem.Should().Be("Meta criada com sucesso.");
        }

        [Fact]
        public async Task CreateAsync_DeveRetornarFalhaQuandoValorMetaMenorOuIgualZero()
        {
            // Arrange
            var dto = new MetaCreateDto
            {
                Nome = "Meta inválida",
                ValorMeta = 0,
                Tipo = TipoMeta.Despesa,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            // Act
            var result = await _service.CreateAsync(dto, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("O valor da meta deve ser maior que zero.");
        }

        [Fact]
        public async Task CreateAsync_DeveRetornarFalhaQuandoDataInicioMaiorQueDataFim()
        {
            // Arrange
            var dto = new MetaCreateDto
            {
                Nome = "Meta inválida",
                ValorMeta = 500,
                Tipo = TipoMeta.Despesa,
                DataInicio = new DateTime(2026, 3, 1),
                DataFim = new DateTime(2026, 2, 1)
            };

            // Act
            var result = await _service.CreateAsync(dto, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("A data de início não pode ser maior que a data de fim.");
        }

        [Fact]
        public async Task GetByIdAsync_DeveCalcularProgressoCorretamenteParaDespesaComCategoria()
        {
            // Arrange
            var meta = new MetaFinanceiraModel
            {
                Id = 1,
                UsuarioId = 10,
                Nome = "Limite alimentação",
                ValorMeta = 500,
                Tipo = TipoMeta.Despesa,
                CategoriaDespesaId = 1,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            var dtoMapeado = new MetaDto { Id = 1, Nome = "Limite alimentação", ValorMeta = 500, Tipo = TipoMeta.Despesa };

            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(meta);
            _mapperMock.Setup(m => m.Map<MetaDto>(meta)).Returns(dtoMapeado);
            _repositoryMock.Setup(r => r.GetSomaDespesasAsync(10, meta.DataInicio, meta.DataFim, 1)).ReturnsAsync(250m);

            // Act
            var result = await _service.GetByIdAsync(1, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados!.ValorAtual.Should().Be(250m);
            result.Dados.Percentual.Should().Be(50m);
            result.Dados.StatusMeta.Should().Be("no_limite");
        }

        [Fact]
        public async Task GetByIdAsync_DeveCalcularProgressoCorretamenteParaReceitaSemCategoria()
        {
            // Arrange
            var meta = new MetaFinanceiraModel
            {
                Id = 2,
                UsuarioId = 10,
                Nome = "Meta receita",
                ValorMeta = 1000,
                Tipo = TipoMeta.Receita,
                CategoriaReceitaId = null,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            var dtoMapeado = new MetaDto { Id = 2, Nome = "Meta receita", ValorMeta = 1000, Tipo = TipoMeta.Receita };

            _repositoryMock.Setup(r => r.GetByIdAsync(2, 10)).ReturnsAsync(meta);
            _mapperMock.Setup(m => m.Map<MetaDto>(meta)).Returns(dtoMapeado);
            _repositoryMock.Setup(r => r.GetSomaReceitasAsync(10, meta.DataInicio, meta.DataFim, null)).ReturnsAsync(800m);

            // Act
            var result = await _service.GetByIdAsync(2, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Dados!.ValorAtual.Should().Be(800m);
            result.Dados.Percentual.Should().Be(80m);
            result.Dados.StatusMeta.Should().Be("em_andamento");
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarStatusExcedidoQuandoDespesasMaiorQueMeta()
        {
            // Arrange
            var meta = new MetaFinanceiraModel
            {
                Id = 3,
                UsuarioId = 10,
                Nome = "Limite lazer",
                ValorMeta = 200,
                Tipo = TipoMeta.Despesa,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            var dtoMapeado = new MetaDto { Id = 3, ValorMeta = 200, Tipo = TipoMeta.Despesa };

            _repositoryMock.Setup(r => r.GetByIdAsync(3, 10)).ReturnsAsync(meta);
            _mapperMock.Setup(m => m.Map<MetaDto>(meta)).Returns(dtoMapeado);
            _repositoryMock.Setup(r => r.GetSomaDespesasAsync(10, meta.DataInicio, meta.DataFim, null)).ReturnsAsync(300m);

            // Act
            var result = await _service.GetByIdAsync(3, 10);

            // Assert
            result.Dados!.StatusMeta.Should().Be("excedido");
            result.Dados.Percentual.Should().Be(150m);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarStatusAtingidaQuandoReceitasMaiorOuIgualMeta()
        {
            // Arrange
            var meta = new MetaFinanceiraModel
            {
                Id = 4,
                UsuarioId = 10,
                Nome = "Meta de renda",
                ValorMeta = 3000,
                Tipo = TipoMeta.Receita,
                DataInicio = new DateTime(2026, 2, 1),
                DataFim = new DateTime(2026, 2, 28)
            };

            var dtoMapeado = new MetaDto { Id = 4, ValorMeta = 3000, Tipo = TipoMeta.Receita };

            _repositoryMock.Setup(r => r.GetByIdAsync(4, 10)).ReturnsAsync(meta);
            _mapperMock.Setup(m => m.Map<MetaDto>(meta)).Returns(dtoMapeado);
            _repositoryMock.Setup(r => r.GetSomaReceitasAsync(10, meta.DataInicio, meta.DataFim, null)).ReturnsAsync(3000m);

            // Act
            var result = await _service.GetByIdAsync(4, 10);

            // Assert
            result.Dados!.StatusMeta.Should().Be("atingida");
            result.Dados.Percentual.Should().Be(100m);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverMetaQuandoEncontrada()
        {
            // Arrange
            var meta = new MetaFinanceiraModel { Id = 1, UsuarioId = 10 };
            _repositoryMock.Setup(r => r.GetByIdAsync(1, 10)).ReturnsAsync(meta);

            // Act
            var result = await _service.DeleteAsync(1, 10);

            // Assert
            result.Status.Should().BeTrue();
            result.Mensagem.Should().Be("Meta removida com sucesso.");
            _repositoryMock.Verify(r => r.DeleteAsync(meta), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarFalhaQuandoMetaNaoEncontrada()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99, 10)).ReturnsAsync((MetaFinanceiraModel?)null);

            // Act
            var result = await _service.DeleteAsync(99, 10);

            // Assert
            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Meta não encontrada.");
        }
    }
}
