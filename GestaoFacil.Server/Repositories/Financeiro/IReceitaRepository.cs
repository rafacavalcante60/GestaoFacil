using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IReceitaRepository
    {
        Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId);
        Task<PagedList<ReceitaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize);
        Task<PagedList<ReceitaModel>> FiltrarPagedAsync(int usuarioId, ReceitaFiltroDto filtro);
        Task<ReceitaModel> AddAsync(ReceitaModel receita);
        Task UpdateAsync(ReceitaModel receita);
        Task DeleteAsync(ReceitaModel receita);
    }
}
