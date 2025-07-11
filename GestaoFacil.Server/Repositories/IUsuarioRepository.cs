using GestaoFacil.Server.Models;

namespace GestaoFacil.Server.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(Usuario usuario);
    }
}
