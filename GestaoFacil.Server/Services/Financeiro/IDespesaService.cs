using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Despesa
{
    public interface IDespesaService
    {
        Task<ResponseModel<PagedList<DespesaDto>>> GetByUsuarioPagedAsync(int usuarioId, Parameters parameters);
        Task<ResponseModel<DespesaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<DespesaDto>> CreateAsync(DespesaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
        Task<ResponseModel<List<DespesaDto>>> FiltrarAsync(DespesaFiltroDto filtro, int usuarioId);
    }
}
