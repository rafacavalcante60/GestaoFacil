using GestaoFacil.Server.Models;

namespace GestaoFacil.Server.Repositories
{
    public interface IDespesaRepository
    {
        Task<List<Despesa>> GetAllByUsuarioAsync(int usuarioId);
        Task<Despesa?> GetByIdAsync(int id, int usuarioId);
        Task AddAsync(Despesa despesa);
        Task UpdateAsync(Despesa despesa);
        Task DeleteAsync(Despesa despesa);
    }
}
