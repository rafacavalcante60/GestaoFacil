using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Receita
{
    public interface IReceitaRepository
    {
            Task<List<ReceitaModel>> GetAllByUsuarioAsync(int usuarioId);
            Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId);
            Task AddAsync(ReceitaModel receita);
            Task UpdateAsync(ReceitaModel receita);
            Task DeleteAsync(ReceitaModel receita);
            Task<bool> ExistsAsync(int id, int usuarioId);
        
    }
}
