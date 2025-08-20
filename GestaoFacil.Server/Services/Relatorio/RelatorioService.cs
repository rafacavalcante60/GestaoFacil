using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Relatorio
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IDespesaRepository _despesaRepo;
        private readonly IReceitaRepository _receitaRepo;
        private readonly ILogger<RelatorioService> _logger;

        public RelatorioService(IDespesaRepository despesaRepo, IReceitaRepository receitaRepo, ILogger<RelatorioService> logger)
        {
            _despesaRepo = despesaRepo;
            _receitaRepo = receitaRepo;
            _logger = logger;
        }

        public async Task<ResponseModel<ResumoFinanceiroDto>> ObterResumoFinanceiroAsync(int usuarioId, DateTime? inicio, DateTime? fim)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<ResumoFinanceiroDto>("A data inicial não pode ser maior que a data final.");
            }

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim });

            var resumo = new ResumoFinanceiroDto
            {
                TotalDespesas = despesas.Sum(d => d.Valor),
                TotalReceitas = receitas.Sum(r => r.Valor)
            };

            return ResponseHelper.Sucesso(resumo, "Resumo financeiro calculado com sucesso.");
        }

        public async Task<ResponseModel<List<CategoriaResumoDto>>> ObterResumoPorCategoriaAsync(int usuarioId, DateTime? inicio, DateTime? fim, bool despesas = true)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<List<CategoriaResumoDto>>("A data inicial não pode ser maior que a data final.");
            }

            if (despesas)
            {
                var lista = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim });
                var grouped = lista
                    .GroupBy(d => d.CategoriaDespesa.Nome)
                    .Select(g => new CategoriaResumoDto { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                    .ToList();

                return ResponseHelper.Sucesso(grouped, "Resumo por categoria de despesas calculado.");
            }
            else
            {
                var lista = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim });
                var grouped = lista
                    .GroupBy(r => r.CategoriaReceita.Nome)
                    .Select(g => new CategoriaResumoDto { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                    .ToList();

                return ResponseHelper.Sucesso(grouped, "Resumo por categoria de receitas calculado.");
            }
        }

        public async Task<ResponseModel<List<FluxoCaixaDto>>> ObterFluxoCaixaAsync(int usuarioId, DateTime? inicio, DateTime? fim)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                _logger.LogWarning("Data inicial {Inicio} maior que Data final {Fim} para usuário {UsuarioId}", inicio, fim, usuarioId);
                return ResponseHelper.Falha<List<FluxoCaixaDto>>("A data inicial não pode ser maior que a data final.");
            }

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim });

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

            return ResponseHelper.Sucesso(fluxo, "Fluxo de caixa calculado com sucesso.");
        }

        public async Task<ResponseModel<List<ResumoMensalDto>>> ObterResumoMensalAsync(int usuarioId, int ano)
        {
            if (ano < 1900 || ano > DateTime.Now.Year)
            {
                _logger.LogWarning("Ano {Ano} inválido para usuário {UsuarioId}", ano, usuarioId);
                return ResponseHelper.Falha<List<ResumoMensalDto>>("Ano inválido.");
            }

            var inicio = new DateTime(ano, 1, 1);
            var fim = new DateTime(ano, 12, 31);

            var despesas = await _despesaRepo.FiltrarAsync(usuarioId, new DespesaFiltroDto { DataInicial = inicio, DataFinal = fim });
            var receitas = await _receitaRepo.FiltrarAsync(usuarioId, new ReceitaFiltroDto { DataInicial = inicio, DataFinal = fim });

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

            return ResponseHelper.Sucesso(resumoMensal, "Resumo mensal calculado com sucesso.");
        }
    }
}
