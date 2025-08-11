using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;

namespace GestaoFacil.Server.Repositories.Despesa
{
    public interface IDespesaRepository
    {
        Task<DespesaModel?> GetByIdAsync(int id, int usuarioId);
        Task<PagedList<DespesaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize);
        Task<PagedList<DespesaModel>> FiltrarPagedAsync(int usuarioId, DespesaFiltroDto filtro);
        Task<DespesaModel> AddAsync(DespesaModel despesa);
        Task UpdateAsync(DespesaModel despesa);
        Task DeleteAsync(DespesaModel despesa);
        
    }
}
