using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Despesa
{
    public interface IDespesaService
    {
        Task<ResponseModel<DespesaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<PagedList<DespesaDto>>> GetByUsuarioPagedAsync(int usuarioId, QueryStringParameters parameters);
        Task<ResponseModel<PagedList<DespesaDto>>> FiltrarPagedAsync(int usuarioId, DespesaFiltroDto filtro);
        Task<ResponseModel<DespesaDto>> CreateAsync(DespesaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
        
    }
}
