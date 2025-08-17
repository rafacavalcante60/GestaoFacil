using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;
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

        public async Task<PagedList<ReceitaModel>> GetByUsuarioIdPagedAsync(int usuarioId, int pageNumber, int pageSize)
        {
            var query = _context.Receitas
               .AsNoTracking()
               .Where(r => r.UsuarioId == usuarioId)
               .OrderByDescending(r => r.Data);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<ReceitaModel>(items, totalCount, pageNumber, pageSize);
        }

        //filtro paginado para exibição
        public async Task<PagedList<ReceitaModel>> FiltrarPagedAsync(int usuarioId, ReceitaFiltroDto filtro)
        {
            var query = _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == usuarioId);

            query = AplicarFiltros(query, filtro);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.Data)
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            return new PagedList<ReceitaModel>(items, totalCount, filtro.PageNumber, filtro.PageSize);
        }

        //filtro não paginado para exportação excel
        public async Task<List<ReceitaModel>> FiltrarAsync(int usuarioId, ReceitaFiltroDto filtro)
        {
            var query = _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.CategoriaReceita)
                .Include(r => r.FormaPagamento)
                .AsQueryable();

            query = AplicarFiltros(query, filtro);

            return await query
                .OrderByDescending(r => r.Data)
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

        private IQueryable<ReceitaModel> AplicarFiltros(IQueryable<ReceitaModel> query, ReceitaFiltroDto filtro)
        {
            if (filtro.ValorMin.HasValue)
                query = query.Where(r => r.Valor >= filtro.ValorMin.Value);

            if (filtro.ValorMax.HasValue)
                query = query.Where(r => r.Valor <= filtro.ValorMax.Value);

            if (filtro.DataInicial.HasValue)
                query = query.Where(r => r.Data >= filtro.DataInicial.Value.Date);

            if (filtro.DataFinal.HasValue)
            {
                var dataFinalInclusiva = filtro.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.Data <= dataFinalInclusiva);
            }

            if (filtro.CategoriaReceitaId.HasValue)
                query = query.Where(r => r.CategoriaReceitaId == filtro.CategoriaReceitaId.Value);

            if (filtro.FormaPagamentoId.HasValue)
                query = query.Where(r => r.FormaPagamentoId == filtro.FormaPagamentoId.Value);

            if (!string.IsNullOrWhiteSpace(filtro.BuscaTexto))
                query = query.Where(r => r.Descricao != null && r.Descricao.Contains(filtro.BuscaTexto));

            return query;
        }

    }
}
