using GestaoFacil.Server.Data;
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
    }

}
