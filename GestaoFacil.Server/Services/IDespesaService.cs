using GestaoFacil.Shared.Dtos;

namespace GestaoFacil.Server.Services
{
    public interface IDespesaService
    {
        Task<List<DespesaDto>> GetAllByUsuarioAsync(int usuarioId);
        Task<DespesaDto?> GetByIdAsync(int id, int usuarioId);
        Task<DespesaDto> CreateAsync(DespesaCreateDto dto, int usuarioId);
        Task<bool> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id, int usuarioId);
    }
}
