using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Financeiro;

namespace GestaoFacil.Server.Repositories.Despesa
{
    public interface IDespesaRepository : IFinanceiroRepository<DespesaModel, DespesaFiltroDto>
    {
        Task<bool> CategoriaAcessivelAsync(int categoriaId, int usuarioId);
    }
}
