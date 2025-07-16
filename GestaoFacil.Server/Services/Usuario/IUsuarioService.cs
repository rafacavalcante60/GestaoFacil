using GestaoFacil.Shared.DTOs.Usuario;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Services.Usuario
{
    public interface IUsuarioService
    {
        Task<ResponseModel<UsuarioDto>> GetByIdAsync(int id);
        Task<ResponseModel<List<UsuarioDto>>> GetAllAsync();
        Task<ResponseModel<bool>> UpdatePerfilAsync(int id, UsuarioUpdateDto dto);
        Task<ResponseModel<bool>> UpdateAdminAsync(int id, UsuarioAdminUpdateDto dto);
        Task<ResponseModel<bool>> DeleteAsync(int id);
    }
}
