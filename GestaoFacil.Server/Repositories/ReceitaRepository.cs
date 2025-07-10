using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories
{
    public class ReceitaRepository : IReceitaRepository
    {
        private readonly AppDbContext _context;

        public ReceitaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receita>> GetAllByUsuarioAsync(int usuarioId)
        {
            return await _context.Receitas
                .Include(r => r.Usuario)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Receita?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Receitas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        }

        public async Task AddAsync(Receita receita)
        {
            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Receita receita)
        {
            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Receita receita)
        {
            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id, int usuarioId)
        {
            return await _context.Receitas.AnyAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        }
    }

}
