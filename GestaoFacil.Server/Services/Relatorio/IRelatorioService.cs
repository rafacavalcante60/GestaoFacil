using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Relatorio
{
    public interface IRelatorioService
    {
        Task<ResponseModel<ResumoFinanceiroDto>> ObterResumoFinanceiroAsync(int usuarioId, DateTime? inicio, DateTime? fim, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null);

        Task<ResponseModel<List<CategoriaResumoDto>>> ObterResumoPorCategoriaAsync(int usuarioId, DateTime? inicio, DateTime? fim, bool despesas = true, int? categoriaId = null, int? formaPagamentoId = null);

        Task<ResponseModel<List<FluxoCaixaDto>>> ObterFluxoCaixaAsync(int usuarioId, DateTime? inicio, DateTime? fim, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null);

        Task<ResponseModel<List<ResumoMensalDto>>> ObterResumoMensalAsync(int usuarioId, int ano, int? categoriaDespesaId = null, int? categoriaReceitaId = null, int? formaPagamentoId = null);
    }
}
