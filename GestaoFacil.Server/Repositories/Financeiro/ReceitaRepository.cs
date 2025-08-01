using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories.Financeiro
{
    public class ReceitaRepository : IReceitaRepository
    {
        private readonly AppDbContext _context;

        public ReceitaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReceitaModel?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Receitas
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        }

        public async Task<List<ReceitaModel>> GetRecentByUsuarioAsync(int usuarioId)
        {
            return await _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(d => d.Data)
                .Take(15)
                .ToListAsync();
        }

        public async Task<ReceitaModel> AddAsync(ReceitaModel receita)
        {
            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();
            return receita;
        }

        public async Task UpdateAsync(ReceitaModel receita)
        {
            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReceitaModel receita)
        {
            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReceitaModel>> FiltrarAsync(ReceitaFiltroDto filtro, int usuarioId)
        {
            var query = _context.Receitas
                .Where(r => r.UsuarioId == usuarioId)
                .AsQueryable();

            if (filtro.ValorMin.HasValue)
            {
                query = query.Where(r => r.Valor >= filtro.ValorMin);
            }

            if (filtro.ValorMax.HasValue)
            {
                query = query.Where(r => r.Valor <= filtro.ValorMax);
            }

            if (filtro.DataInicial.HasValue)
            {
                query = query.Where(r => r.Data >= filtro.DataInicial.Value.Date);
            }

            if (filtro.DataFinal.HasValue)
            {
                var dataFinalInclusiva = filtro.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.Data <= dataFinalInclusiva);
            }

            if (filtro.CategoriaReceitaId.HasValue)
            {
                query = query.Where(r => r.CategoriaReceitaId == filtro.CategoriaReceitaId);
            }

            if (filtro.FormaPagamentoId.HasValue)
            {
                query = query.Where(r => r.FormaPagamentoId == filtro.FormaPagamentoId);
            }

            if (!string.IsNullOrWhiteSpace(filtro.BuscaTexto))
            {
                query = query.Where(r => r.Descricao != null && r.Descricao.Contains(filtro.BuscaTexto));
            }

            return await query
                .OrderByDescending(r => r.Data)
                .ToListAsync();
        }

    }
}
