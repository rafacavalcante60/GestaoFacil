using GestaoFacil.Server.Data;
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

        public async Task<List<DespesaModel>> GetAllByUsuarioAsync(int usuarioId)
        {
            return await _context.Despesas
                .Where(d => d.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<DespesaModel?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Despesas.FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);
        }

        public async Task AddAsync(DespesaModel despesa)
        {
            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
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
    }
}
