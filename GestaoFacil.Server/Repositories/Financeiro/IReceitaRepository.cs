using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IReceitaRepository
    {
            Task<List<ReceitaModel>> GetRecentByUsuarioAsync(int usuarioId);
            Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId);
            Task AddAsync(ReceitaModel receita);
            Task UpdateAsync(ReceitaModel receita);
            Task DeleteAsync(ReceitaModel receita);
    }
}
