using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories
{
    public class DespesaRepository : IDespesaRepository
    {
        private readonly AppDbContext _context;

        public DespesaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Despesa>> GetAllByUsuarioAsync(int usuarioId)
        {
            return await _context.Despesas
                .Include(d => d.Usuario)
                .Where(d => d.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Despesa?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Despesas
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);
        }

        public async Task AddAsync(Despesa despesa)
        {
            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Despesa despesa)
        {
            _context.Despesas.Update(despesa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Despesa despesa)
        {
            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();
        }
    }
}
