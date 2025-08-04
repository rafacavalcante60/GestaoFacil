using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories.Despesa
{
    public class DespesaRepository : IDespesaRepository
    {
        private readonly AppDbContext _context;

        public DespesaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DespesaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize)
        {
            return await _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.Data)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<DespesaModel?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Despesas
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);
        }

        public async Task<DespesaModel> AddAsync(DespesaModel despesa)
        {
            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
            return despesa;
        }

        public async Task UpdateAsync(DespesaModel despesa)
        {
            _context.Despesas.Update(despesa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DespesaModel despesa)
        {
            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DespesaModel>> FiltrarAsync(DespesaFiltroDto filtro, int usuarioId)
        {
            var query = _context.Despesas
                .Where(d => d.UsuarioId == usuarioId)
                .AsQueryable();

            if (filtro.ValorMin.HasValue)
            {
                query = query.Where(d => d.Valor >= filtro.ValorMin);
            }

            if (filtro.ValorMax.HasValue)
            {
                query = query.Where(d => d.Valor <= filtro.ValorMax);
            }

            if (filtro.DataInicial.HasValue)
            {
                query = query.Where(d => d.Data >= filtro.DataInicial.Value.Date); //00:00:00
            }

            if (filtro.DataFinal.HasValue)
            {
                //(23:59:59.999)
                var dataFinalInclusiva = filtro.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.Data <= dataFinalInclusiva);
            }

            if (filtro.CategoriaDespesaId.HasValue)
            {
                query = query.Where(d => d.CategoriaDespesaId == filtro.CategoriaDespesaId);
            }

            if (filtro.FormaPagamentoId.HasValue)
            {
                query = query.Where(d => d.FormaPagamentoId == filtro.FormaPagamentoId);
            }

            if (!string.IsNullOrWhiteSpace(filtro.BuscaTexto))
            {
                query = query.Where(d => d.Descricao != null && d.Descricao.Contains(filtro.BuscaTexto));
            }

            return await query
                .OrderByDescending(d => d.Data)
                .ToListAsync();
        }

    }
}