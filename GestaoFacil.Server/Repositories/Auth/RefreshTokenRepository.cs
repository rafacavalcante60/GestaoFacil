using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Repositories.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshTokenModel token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshTokenModel?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<RefreshTokenModel?> GetValidByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(t => t.Usuario)
                    .ThenInclude(u => u.TipoUsuario)
                .FirstOrDefaultAsync(t => t.Token == token && !t.EstaRevogado);
        }

        // Revoga em lote no banco: quem troca de senha precisa expulsar as sessoes antigas,
        // senao um refresh token roubado continua valido pelos 7 dias originais.
        public async Task RevokeAllByUsuarioAsync(int usuarioId)
        {
            await _context.RefreshTokens
                .Where(t => t.UsuarioId == usuarioId && !t.EstaRevogado)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.EstaRevogado, true));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
