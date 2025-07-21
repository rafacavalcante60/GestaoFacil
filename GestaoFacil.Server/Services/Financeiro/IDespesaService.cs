using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Services.Despesa
{
    public interface IDespesaService
    {
        Task<ResponseModel<List<DespesaDto>>> GetAllByUsuarioAsync(int usuarioId);
        Task<ResponseModel<DespesaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<DespesaDto>> CreateAsync(DespesaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
    }
}
