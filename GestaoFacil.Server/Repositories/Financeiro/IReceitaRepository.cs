using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public interface IReceitaRepository : IFinanceiroRepository<ReceitaModel, ReceitaFiltroDto>
    {
    }
}
