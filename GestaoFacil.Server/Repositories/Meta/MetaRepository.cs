using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models.Principais;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories.Meta
{
    public class MetaRepository : IMetaRepository
    {
        private readonly AppDbContext _context;

        public MetaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MetaFinanceiraModel?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.MetasFinanceiras
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);
        }

        public async Task<List<MetaFinanceiraModel>> GetByUsuarioAsync(int usuarioId)
        {
            return await _context.MetasFinanceiras
                .AsNoTracking()
                .Where(m => m.UsuarioId == usuarioId)
                .OrderByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        public async Task<List<MetaFinanceiraModel>> GetAtivasAsync(int usuarioId)
        {
            return await _context.MetasFinanceiras
                .AsNoTracking()
                .Where(m => m.UsuarioId == usuarioId && m.Ativo)
                .OrderByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        public async Task<MetaFinanceiraModel> AddAsync(MetaFinanceiraModel meta)
        {
            _context.MetasFinanceiras.Add(meta);
            await _context.SaveChangesAsync();
            return meta;
        }

        public async Task UpdateAsync(MetaFinanceiraModel meta)
        {
            _context.MetasFinanceiras.Update(meta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MetaFinanceiraModel meta)
        {
            _context.MetasFinanceiras.Remove(meta);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetSomaDespesasAsync(int usuarioId, DateTime dataInicio, DateTime dataFim, int? categoriaId)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == usuarioId && d.Data >= dataInicio && d.Data <= dataFim);

            if (categoriaId.HasValue)
                query = query.Where(d => d.CategoriaDespesaId == categoriaId.Value);

            return await query.SumAsync(d => (decimal?)d.Valor) ?? 0m;
        }

        public async Task<decimal> GetSomaReceitasAsync(int usuarioId, DateTime dataInicio, DateTime dataFim, int? categoriaId)
        {
            var query = _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == usuarioId && r.Data >= dataInicio && r.Data <= dataFim);

            if (categoriaId.HasValue)
                query = query.Where(r => r.CategoriaReceitaId == categoriaId.Value);

            return await query.SumAsync(r => (decimal?)r.Valor) ?? 0m;
        }
    }
}
