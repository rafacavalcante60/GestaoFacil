using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Financeiro
{
    public interface IReceitaService
    {
        Task<ResponseModel<ReceitaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<PagedList<ReceitaDto>>> GetByUsuarioPagedAsync(int usuarioId, QueryStringParameters parameters);
        Task<ResponseModel<PagedList<ReceitaDto>>> FiltrarPagedAsync(int usuarioId, ReceitaFiltroDto filtro);
        Task<ResponseModel<byte[]>> ExportarExcelCompletoAsync(int usuarioId, ReceitaFiltroDto filtro);
        Task<ResponseModel<ReceitaDto>> CreateAsync(ReceitaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
    }
}
