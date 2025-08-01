using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IReceitaRepository
    {
        Task<List<ReceitaModel>> GetRecentByUsuarioAsync(int usuarioId);
        Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId);
        Task<ReceitaModel> AddAsync(ReceitaModel receita);
        Task UpdateAsync(ReceitaModel receita);
        Task DeleteAsync(ReceitaModel receita);
        Task<List<ReceitaModel>> FiltrarAsync(ReceitaFiltroDto filtro, int usuarioId);
    }
}
