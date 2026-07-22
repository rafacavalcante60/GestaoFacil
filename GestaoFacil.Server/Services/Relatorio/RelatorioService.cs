using System.Globalization;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Cache;

namespace GestaoFacil.Server.Services.Relatorio
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IDespesaRepository _despesaRepo;
        private readonly IReceitaRepository _receitaRepo;
        private readonly IRelatorioCache _cache;
        private readonly ILogger<RelatorioService> _logger;

        public RelatorioService(IDespesaRepository despesaRepo, IReceitaRepository receitaRepo, IRelatorioCache cache, ILogger<RelatorioService> logger)
        {
            _despesaRepo = despesaRepo;
            _receitaRepo = receitaRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ResponseModel<ResumoFinanceiroDto>> ObterResumoFinanceiroAsync(int usuarioId, DateTime? inicio, DateTime? fim, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<ResumoFinanceiroDto>("A data inicial não pode ser maior que a data final.");
            }

            var chave = MontarChave("resumo", inicio, fim, categoriaDespesaId, categoriaReceitaId, formaPagamentoId);
            var cacheado = await _cache.ObterAsync<ResumoFinanceiroDto>(usuarioId, chave);
            if (cacheado is not null)
            {
                return ResponseHelper.Sucesso(cacheado, "Resumo financeiro calculado com sucesso.");
            }

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaDespesaId = categoriaDespesaId, FormaPagamentoId = formaPagamentoId });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaReceitaId = categoriaReceitaId, FormaPagamentoId = formaPagamentoId });

            var resumo = new ResumoFinanceiroDto
            {
                TotalDespesas = despesas.Sum(d => d.Valor),
                TotalReceitas = receitas.Sum(r => r.Valor)
            };

            await _cache.GravarAsync(usuarioId, chave, resumo);
            return ResponseHelper.Sucesso(resumo, "Resumo financeiro calculado com sucesso.");
        }

        public async Task<ResponseModel<List<CategoriaResumoDto>>> ObterResumoPorCategoriaAsync(int usuarioId, DateTime? inicio, DateTime? fim, bool despesas = true, int? categoriaId = null, int? formaPagamentoId = null)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<List<CategoriaResumoDto>>("A data inicial não pode ser maior que a data final.");
            }

            var chave = MontarChave($"categoria:{despesas}", inicio, fim, categoriaId, null, formaPagamentoId);
            var cacheado = await _cache.ObterAsync<List<CategoriaResumoDto>>(usuarioId, chave);
            if (cacheado is not null)
            {
                var msgCache = despesas ? "Resumo por categoria de despesas calculado." : "Resumo por categoria de receitas calculado.";
                return ResponseHelper.Sucesso(cacheado, msgCache);
            }

            if (despesas)
            {
                var lista = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaDespesaId = categoriaId, FormaPagamentoId = formaPagamentoId });
                var grouped = lista
                    .GroupBy(d => d.CategoriaDespesa.Nome)
                    .Select(g => new CategoriaResumoDto { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                    .ToList();

                await _cache.GravarAsync(usuarioId, chave, grouped);
                return ResponseHelper.Sucesso(grouped, "Resumo por categoria de despesas calculado.");
            }
            else
            {
                var lista = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaReceitaId = categoriaId, FormaPagamentoId = formaPagamentoId });
                var grouped = lista
                    .GroupBy(r => r.CategoriaReceita.Nome)
                    .Select(g => new CategoriaResumoDto { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                    .ToList();

                await _cache.GravarAsync(usuarioId, chave, grouped);
                return ResponseHelper.Sucesso(grouped, "Resumo por categoria de receitas calculado.");
            }
        }

        public async Task<ResponseModel<List<FluxoCaixaDto>>> ObterFluxoCaixaAsync(int usuarioId, DateTime? inicio, DateTime? fim, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<List<FluxoCaixaDto>>("A data inicial não pode ser maior que a data final.");
            }

            var chave = MontarChave("fluxo", inicio, fim, categoriaDespesaId, categoriaReceitaId, formaPagamentoId);
            var cacheado = await _cache.ObterAsync<List<FluxoCaixaDto>>(usuarioId, chave);
            if (cacheado is not null)
            {
                return ResponseHelper.Sucesso(cacheado, "Fluxo de caixa calculado com sucesso.");
            }

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaDespesaId = categoriaDespesaId, FormaPagamentoId = formaPagamentoId });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaReceitaId = categoriaReceitaId, FormaPagamentoId = formaPagamentoId });

            var datas = despesas.Select(d => d.Data.Date)
                                .Union(receitas.Select(r => r.Data.Date))
                                .Distinct()
                                .OrderBy(d => d)
                                .ToList();

            var fluxo = new List<FluxoCaixaDto>();
            decimal saldoAcumulado = 0;

            foreach (var data in datas)
            {
                var totalReceita = receitas.Where(r => r.Data.Date == data).Sum(r => r.Valor);
                var totalDespesa = despesas.Where(d => d.Data.Date == data).Sum(d => d.Valor);
                saldoAcumulado += (totalReceita - totalDespesa);

                fluxo.Add(new FluxoCaixaDto { Data = data, SaldoAcumulado = saldoAcumulado });
            }

            await _cache.GravarAsync(usuarioId, chave, fluxo);
            return ResponseHelper.Sucesso(fluxo, "Fluxo de caixa calculado com sucesso.");
        }

        public async Task<ResponseModel<List<ResumoMensalDto>>> ObterResumoMensalAsync(int usuarioId, int ano, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null)
        {
            if (ano < 1900 || ano > DateTime.Now.Year)
            {
                _logger.LogWarning("Ano {Ano} inválido para usuário {UsuarioId}", ano, usuarioId);
                return ResponseHelper.Falha<List<ResumoMensalDto>>("Ano inválido.");
            }

            var chave = $"mensal:{ano}:{categoriaDespesaId}:{categoriaReceitaId}:{formaPagamentoId}";
            var cacheado = await _cache.ObterAsync<List<ResumoMensalDto>>(usuarioId, chave);
            if (cacheado is not null)
            {
                return ResponseHelper.Sucesso(cacheado, "Resumo mensal calculado com sucesso.");
            }

            var inicio = new DateTime(ano, 1, 1);
            var fim = new DateTime(ano, 12, 31);

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaDespesaId = categoriaDespesaId, FormaPagamentoId = formaPagamentoId });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim, CategoriaReceitaId = categoriaReceitaId, FormaPagamentoId = formaPagamentoId });

            var resumoMensal = Enumerable.Range(1, 12)
                .Select(mes =>
                {
                    var totalReceitas = receitas.Where(r => r.Data.Month == mes).Sum(r => r.Valor);
                    var totalDespesas = despesas.Where(d => d.Data.Month == mes).Sum(d => d.Valor);
                    return new ResumoMensalDto
                    {
                        Mes = mes,
                        TotalReceitas = totalReceitas,
                        TotalDespesas = totalDespesas,
                        Saldo = totalReceitas - totalDespesas
                    };
                })
                .ToList();

            await _cache.GravarAsync(usuarioId, chave, resumoMensal);
            return ResponseHelper.Sucesso(resumoMensal, "Resumo mensal calculado com sucesso.");
        }

        // Monta uma chave determinística a partir dos filtros. Usa formato invariante para a
        // data (o "o" = ISO-8601), senão a cultura do servidor mudaria a chave e provocaria miss.
        private static string MontarChave(string prefixo, DateTime? inicio, DateTime? fim, int? catDespesa, int? catReceita, int? formaPagamento)
        {
            var i = inicio?.ToString("o", CultureInfo.InvariantCulture) ?? "_";
            var f = fim?.ToString("o", CultureInfo.InvariantCulture) ?? "_";
            return $"{prefixo}:{i}:{f}:{catDespesa}:{catReceita}:{formaPagamento}";
        }
    }
}
