using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Repositories.Meta
{
    public interface IMetaRepository
    {
        Task<MetaFinanceiraModel?> GetByIdAsync(int id, int usuarioId);
        Task<List<MetaFinanceiraModel>> GetByUsuarioAsync(int usuarioId);
        Task<List<MetaFinanceiraModel>> GetAtivasAsync(int usuarioId);
        Task<MetaFinanceiraModel> AddAsync(MetaFinanceiraModel meta);
        Task UpdateAsync(MetaFinanceiraModel meta);
        Task DeleteAsync(MetaFinanceiraModel meta);
        Task<decimal> GetSomaDespesasAsync(int usuarioId, DateTime dataInicio, DateTime dataFim, int? categoriaId);
        Task<decimal> GetSomaReceitasAsync(int usuarioId, DateTime dataInicio, DateTime dataFim, int? categoriaId);
    }
}
