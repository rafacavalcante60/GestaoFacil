using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Despesa
{
    public interface IDespesaRepository
    {
        Task<List<DespesaModel>> GetAllByUsuarioAsync(int usuarioId);
        Task<DespesaModel?> GetByIdAsync(int id, int usuarioId);
        Task AddAsync(DespesaModel despesa);
        Task UpdateAsync(DespesaModel despesa);
        Task DeleteAsync(DespesaModel despesa);
    }
}
