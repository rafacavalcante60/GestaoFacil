using GestaoFacil.Server.Models.Auth;

namespace GestaoFacil.Server.Repositories.Auth
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshTokenModel token);
        Task<RefreshTokenModel?> GetByTokenAsync(string token);
        Task<RefreshTokenModel?> GetValidByTokenAsync(string token);
        Task SaveChangesAsync();
    }
}
