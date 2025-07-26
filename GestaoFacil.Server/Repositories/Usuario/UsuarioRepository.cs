using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models.Usuario;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioModel?> GetByIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Id == id);

            return usuario;
        }

        public async Task<List<UsuarioModel>> GetRecentAsync()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Take(15)
                .ToListAsync();
        }

        public async Task AddAsync(UsuarioModel usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UsuarioModel usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UsuarioModel usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<TipoUsuarioModel?> GetTipoUsuarioByNameAsync(string nome)
        {
            var tipo = await _context.TiposUsuario
                .FirstOrDefaultAsync(t => t.Nome == nome && t.Ativo);

            return tipo;
        }

        public async Task<UsuarioModel?> GetByEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                                        .Include(u => u.TipoUsuario)
                                        .FirstOrDefaultAsync(u => u.Email == email);

            return usuario;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}
