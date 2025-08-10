using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IReceitaRepository
    {
        Task<PagedList<ReceitaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize);
        Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId);
        Task<ReceitaModel> AddAsync(ReceitaModel receita);
        Task UpdateAsync(ReceitaModel receita);
        Task DeleteAsync(ReceitaModel receita);
        Task<List<ReceitaModel>> FiltrarAsync(ReceitaFiltroDto filtro, int usuarioId);
    }
}
