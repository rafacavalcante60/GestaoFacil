using GestaoFacil.Server.Responses;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.DTOs.Filtro;

namespace GestaoFacil.Server.Services.Financeiro
{
    public interface IReceitaService
    {
        Task<ResponseModel<ReceitaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<List<ReceitaDto>>> GetRecentByUsuarioAsync(int usuarioId);
        Task<ResponseModel<ReceitaDto>> CreateAsync(ReceitaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
        Task<ResponseModel<List<ReceitaDto>>> FiltrarAsync(ReceitaFiltroDto filtro, int usuarioId);
    }
}
