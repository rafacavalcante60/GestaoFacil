using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Relatorio
{
    public interface IRelatorioService
    {
        Task<ResponseModel<ResumoFinanceiroDto>> ObterResumoFinanceiroAsync(int usuarioId, DateTime? inicio, DateTime? fim);

        Task<ResponseModel<List<CategoriaResumoDto>>> ObterResumoPorCategoriaAsync(int usuarioId, DateTime? inicio, DateTime? fim, bool despesas = true);

        Task<ResponseModel<List<FluxoCaixaDto>>> ObterFluxoCaixaAsync(int usuarioId, DateTime? inicio, DateTime? fim);

        Task<ResponseModel<List<ResumoMensalDto>>> ObterResumoMensalAsync(int usuarioId, int ano);
    }
}
