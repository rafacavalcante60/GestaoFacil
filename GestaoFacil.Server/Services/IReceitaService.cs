using GestaoFacil.Shared.Dtos;

namespace GestaoFacil.Server.Services
{
    public interface IReceitaService
    {
        Task<List<ReceitaDto>> GetAllByUsuarioAsync(int usuarioId);
        Task<ReceitaDto?> GetByIdAsync(int id, int usuarioId);
        Task<ReceitaDto> CreateAsync(ReceitaCreateDto dto, int usuarioId);
        Task<bool> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id, int usuarioId);
    }
}