using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Pagination;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IFinanceiroRepository<TModel, TFiltro>
        where TModel : class
        where TFiltro : IFiltroData
    {
        Task<TModel?> GetByIdAsync(int id, int usuarioId);
        Task<PagedList<TModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize);
        Task<PagedList<TModel>> FiltrarPagedAsync(int usuarioId, TFiltro filtro);
        Task<List<TModel>> FiltrarAsync(int usuarioId, TFiltro filtro);
        Task<TModel> AddAsync(TModel entity);
        Task UpdateAsync(TModel entity);
        Task DeleteAsync(TModel entity);
    }
}
