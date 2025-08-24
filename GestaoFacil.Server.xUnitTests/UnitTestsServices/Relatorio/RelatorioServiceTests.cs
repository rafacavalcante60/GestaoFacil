using FluentAssertions;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Services.Relatorio;
using Microsoft.Extensions.Logging;
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
        private readonly RelatorioService _service;

        public RelatorioServiceTests()
        {
            _despesaRepoMock = new Mock<IDespesaRepository>();
            _receitaRepoMock = new Mock<IReceitaRepository>();
            _loggerMock = new Mock<ILogger<RelatorioService>>();

            _service = new RelatorioService(
                _despesaRepoMock.Object,
                _receitaRepoMock.Object,
                _loggerMock.Object
            );
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
    }
}