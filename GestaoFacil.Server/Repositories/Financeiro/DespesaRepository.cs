using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;
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

        public async Task<DespesaModel?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Despesas
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);
        }

        public async Task<PagedList<DespesaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.Data);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<DespesaModel>(items, totalCount, pageNumber, pageSize);
        }

        //filtro paginado para exibição
        public async Task<PagedList<DespesaModel>> FiltrarPagedAsync(int usuarioId, DespesaFiltroDto filtro)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == usuarioId);

            query = AplicarFiltros(query, filtro);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.Data)
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            return new PagedList<DespesaModel>(items, totalCount, filtro.PageNumber, filtro.PageSize);
        }

        //filtro não paginado para exportação excel
        public async Task<List<DespesaModel>> FiltrarAsync(int usuarioId, DespesaFiltroDto filtro)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == usuarioId)
                .Include(d => d.CategoriaDespesa)
                .Include(d => d.FormaPagamento)
                .AsQueryable();

            query = AplicarFiltros(query, filtro);

            return await query
                .OrderByDescending(d => d.Data)
                .ToListAsync();
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

        private IQueryable<DespesaModel> AplicarFiltros(IQueryable<DespesaModel> query, DespesaFiltroDto filtro)
        {
            if (filtro.ValorMin.HasValue)
                query = query.Where(d => d.Valor >= filtro.ValorMin.Value);

            if (filtro.ValorMax.HasValue)
                query = query.Where(d => d.Valor <= filtro.ValorMax.Value);

            if (filtro.DataInicial.HasValue)
                query = query.Where(d => d.Data >= filtro.DataInicial.Value.Date);

            if (filtro.DataFinal.HasValue)
            {
                var dataFinalInclusiva = filtro.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.Data <= dataFinalInclusiva);
            }

            if (filtro.CategoriaDespesaId.HasValue)
                query = query.Where(d => d.CategoriaDespesaId == filtro.CategoriaDespesaId.Value);

            if (filtro.FormaPagamentoId.HasValue)
                query = query.Where(d => d.FormaPagamentoId == filtro.FormaPagamentoId.Value);

            if (!string.IsNullOrWhiteSpace(filtro.BuscaTexto))
                query = query.Where(d => d.Descricao != null && d.Descricao.Contains(filtro.BuscaTexto));

            return query;
        }
    }
}