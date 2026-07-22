using FluentAssertions;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Services.Cache;
using GestaoFacil.Server.Services.Relatorio;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GestaoFacil.Tests.Services
{
    public class RelatorioServiceTests
    {
        private readonly Mock<IDespesaRepository> _despesaRepoMock;
        private readonly Mock<IReceitaRepository> _receitaRepoMock;
        private readonly Mock<ILogger<RelatorioService>> _loggerMock;
        private readonly IRelatorioCache _cache;
        private readonly RelatorioService _service;

        public RelatorioServiceTests()
        {
            _despesaRepoMock = new Mock<IDespesaRepository>();
            _receitaRepoMock = new Mock<IReceitaRepository>();
            _loggerMock = new Mock<ILogger<RelatorioService>>();
            // Cache real backed por memoria: exercita de verdade serializacao, chave e
            // invalidacao, sem precisar de um Redis rodando nos testes.
            _cache = CriarCacheReal();

            _service = new RelatorioService(
                _despesaRepoMock.Object,
                _receitaRepoMock.Object,
                _cache,
                _loggerMock.Object
            );
        }

        private static IRelatorioCache CriarCacheReal()
        {
            var distributed = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));
            var config = new ConfigurationBuilder().Build();
            return new RelatorioCache(distributed, config, Mock.Of<ILogger<RelatorioCache>>());
        }

        [Fact]
        public async Task ObterResumoFinanceiroAsync_DeveRetornarFalha_QuandoInicioMaiorQueFim()
        {
            var inicio = new DateTime(2025, 1, 2);
            var fim = new DateTime(2025, 1, 1);

            var result = await _service.ObterResumoFinanceiroAsync(1, inicio, fim);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("A data inicial não pode ser maior que a data final.");
        }

        [Fact]
        public async Task ObterResumoFinanceiroAsync_DeveRetornarSucesso_QuandoDatasValidas()
        {
            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel>
                {
                    new() { Valor = 100 },
                    new() { Valor = 50 }
                });

            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel>
                {
                    new() { Valor = 200 },
                    new() { Valor = 100 }
                });

            var result = await _service.ObterResumoFinanceiroAsync(1, DateTime.Now.AddDays(-1), DateTime.Now);

            result.Status.Should().BeTrue();
            result.Dados!.TotalDespesas.Should().Be(150);
            result.Dados.TotalReceitas.Should().Be(300);
            result.Mensagem.Should().Be("Resumo financeiro calculado com sucesso.");
        }

        [Fact]
        public async Task ObterResumoPorCategoriaAsync_DeveRetornarFalha_QuandoInicioMaiorQueFim()
        {
            var inicio = new DateTime(2025, 1, 2);
            var fim = new DateTime(2025, 1, 1);

            var result = await _service.ObterResumoPorCategoriaAsync(1, inicio, fim, true);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("A data inicial não pode ser maior que a data final.");
        }

        [Fact]
        public async Task ObterResumoPorCategoriaAsync_DeveRetornarResumoDeDespesas()
        {
            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel>
                {
                    new() { Valor = 100, CategoriaDespesa = new CategoriaDespesaModel { Nome = "Alimentação" } },
                    new() { Valor = 50, CategoriaDespesa = new CategoriaDespesaModel { Nome = "Alimentação" } },
                    new() { Valor = 30, CategoriaDespesa = new CategoriaDespesaModel { Nome = "Transporte" } }
                });

            var result = await _service.ObterResumoPorCategoriaAsync(1, null, null, despesas: true);

            result.Status.Should().BeTrue();
            result.Dados!.Count.Should().Be(2);
            result.Dados.Should().ContainSingle(x => x.Categoria == "Alimentação" && x.Total == 150);
            result.Dados.Should().ContainSingle(x => x.Categoria == "Transporte" && x.Total == 30);
            result.Mensagem.Should().Be("Resumo por categoria de despesas calculado.");
        }

        [Fact]
        public async Task ObterResumoPorCategoriaAsync_DeveRetornarResumoDeReceitas()
        {
            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel>
                {
                    new() { Valor = 200, CategoriaReceita = new CategoriaReceitaModel { Nome = "Salário" } },
                    new() { Valor = 100, CategoriaReceita = new CategoriaReceitaModel { Nome = "Salário" } },
                    new() { Valor = 50, CategoriaReceita = new CategoriaReceitaModel { Nome = "Investimento" } }
                });

            var result = await _service.ObterResumoPorCategoriaAsync(1, null, null, despesas: false);

            result.Status.Should().BeTrue();
            result.Dados!.Count.Should().Be(2);
            result.Dados.Should().ContainSingle(x => x.Categoria == "Salário" && x.Total == 300);
            result.Dados.Should().ContainSingle(x => x.Categoria == "Investimento" && x.Total == 50);
            result.Mensagem.Should().Be("Resumo por categoria de receitas calculado.");
        }

        [Fact]
        public async Task ObterFluxoCaixaAsync_DeveRetornarFalha_QuandoInicioMaiorQueFim()
        {
            var inicio = new DateTime(2025, 1, 2);
            var fim = new DateTime(2025, 1, 1);

            var result = await _service.ObterFluxoCaixaAsync(1, inicio, fim);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("A data inicial não pode ser maior que a data final.");
        }

        [Fact]
        public async Task ObterFluxoCaixaAsync_DeveCalcularSaldoAcumulado()
        {
            var hoje = DateTime.Today;
            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel>
                {
                    new() { Valor = 50, Data = hoje },
                    new() { Valor = 30, Data = hoje.AddDays(1) }
                });

            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel>
                {
                    new() { Valor = 100, Data = hoje },
                    new() { Valor = 20, Data = hoje.AddDays(1) }
                });

            var result = await _service.ObterFluxoCaixaAsync(1, hoje, hoje.AddDays(1));

            result.Status.Should().BeTrue();
            result.Dados![0].SaldoAcumulado.Should().Be(50); // 100 - 50
            result.Dados[1].SaldoAcumulado.Should().Be(40); // 50 + (20 - 30)
            result.Mensagem.Should().Be("Fluxo de caixa calculado com sucesso.");
        }

        [Fact]
        public async Task ObterResumoMensalAsync_DeveRetornarFalha_QuandoAnoInvalido()
        {
            var result = await _service.ObterResumoMensalAsync(1, 1800);

            result.Status.Should().BeFalse();
            result.Mensagem.Should().Be("Ano inválido.");
        }

        [Fact]
        public async Task ObterResumoMensalAsync_DeveRetornarResumoMensal()
        {
            var ano = DateTime.Now.Year;
            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel>
                {
                    new() { Valor = 50, Data = new DateTime(ano, 1, 10) },
                    new() { Valor = 30, Data = new DateTime(ano, 2, 15) }
                });

            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel>
                {
                    new() { Valor = 100, Data = new DateTime(ano, 1, 5) },
                    new() { Valor = 20, Data = new DateTime(ano, 2, 20) }
                });

            var result = await _service.ObterResumoMensalAsync(1, ano);

            result.Status.Should().BeTrue();
            result.Dados!.Count.Should().Be(12);
            result.Dados[0].Mes.Should().Be(1);
            result.Dados[0].TotalReceitas.Should().Be(100);
            result.Dados[0].TotalDespesas.Should().Be(50);
            result.Dados[0].Saldo.Should().Be(50);

            result.Dados[1].Mes.Should().Be(2);
            result.Dados[1].TotalReceitas.Should().Be(20);
            result.Dados[1].TotalDespesas.Should().Be(30);
            result.Dados[1].Saldo.Should().Be(-10);

            result.Mensagem.Should().Be("Resumo mensal calculado com sucesso.");
        }

        // --- Cache (Fase 5b / Redis) ---

        [Fact]
        public async Task ObterResumoFinanceiroAsync_DeveConsultarBancoUmaVez_QuandoChamadoDuasVezes()
        {
            var inicio = new DateTime(2025, 1, 1);
            var fim = new DateTime(2025, 1, 31);

            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel> { new() { Valor = 100 } });
            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel> { new() { Valor = 300 } });

            var primeira = await _service.ObterResumoFinanceiroAsync(1, inicio, fim);
            var segunda = await _service.ObterResumoFinanceiroAsync(1, inicio, fim);

            // segunda chamada veio do cache: mesmo resultado, mas o banco so foi consultado uma vez
            segunda.Dados!.Saldo.Should().Be(primeira.Dados!.Saldo);
            _despesaRepoMock.Verify(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()), Times.Once);
            _receitaRepoMock.Verify(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()), Times.Once);
        }

        [Fact]
        public async Task ObterResumoFinanceiroAsync_DeveIsolarCachePorUsuario()
        {
            _despesaRepoMock.Setup(r => r.FiltrarAsync(It.IsAny<int>(), It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel> { new() { Valor = 100 } });
            _receitaRepoMock.Setup(r => r.FiltrarAsync(It.IsAny<int>(), It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel> { new() { Valor = 300 } });

            await _service.ObterResumoFinanceiroAsync(1, null, null);
            await _service.ObterResumoFinanceiroAsync(2, null, null);

            // usuarios diferentes nao compartilham cache: cada um consulta o banco
            _despesaRepoMock.Verify(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()), Times.Once);
            _despesaRepoMock.Verify(r => r.FiltrarAsync(2, It.IsAny<DespesaFiltroDto>()), Times.Once);
        }

        [Fact]
        public async Task ObterResumoFinanceiroAsync_DeveRecalcular_AposInvalidarCache()
        {
            _despesaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()))
                .ReturnsAsync(new List<DespesaModel> { new() { Valor = 100 } });
            _receitaRepoMock.Setup(r => r.FiltrarAsync(1, It.IsAny<ReceitaFiltroDto>()))
                .ReturnsAsync(new List<ReceitaModel> { new() { Valor = 300 } });

            await _service.ObterResumoFinanceiroAsync(1, null, null);

            // simula o que Create/Update/Delete de despesa/receita fazem
            await _cache.InvalidarAsync(1);

            await _service.ObterResumoFinanceiroAsync(1, null, null);

            // depois de invalidar, a chamada seguinte tem que bater no banco de novo
            _despesaRepoMock.Verify(r => r.FiltrarAsync(1, It.IsAny<DespesaFiltroDto>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ObterResumoFinanceiroAsync_NaoDeveCachearFalha()
        {
            var inicio = new DateTime(2025, 2, 1);
            var fim = new DateTime(2025, 1, 1); // inicio > fim => falha, nao pode ser cacheada

            var falha = await _service.ObterResumoFinanceiroAsync(1, inicio, fim);
            falha.Status.Should().BeFalse();

            var valor = await _cache.ObterAsync<GestaoFacil.Server.DTOs.Relatorio.ResumoFinanceiroDto>(
                1, $"resumo:{inicio:o}:{fim:o}:::");
            valor.Should().BeNull();
        }
    }
}