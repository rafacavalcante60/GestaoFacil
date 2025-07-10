using GestaoFacil.Server.Models;

namespace GestaoFacil.Server.Repositories
{
    public interface IReceitaRepository
    {
            Task<List<Receita>> GetAllByUsuarioAsync(int usuarioId);
            Task<Receita?> GetByIdAsync(int id, int usuarioId);
            Task AddAsync(Receita receita);
            Task UpdateAsync(Receita receita);
            Task DeleteAsync(Receita receita);
            Task<bool> ExistsAsync(int id, int usuarioId);
        
    }
}
