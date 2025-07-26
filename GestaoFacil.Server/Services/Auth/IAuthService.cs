using GestaoFacil.Server.DTOs.Auth;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Auth
{
    public interface IAuthService
    {
        Task<ResponseModel<TokenDto>> LoginAsync(UsuarioLoginDto request);
        Task<ResponseModel<string>> LogoutAsync(string refreshToken);
        Task<ResponseModel<TokenDto>> RefreshTokenAsync(string refreshToken);
        Task<ResponseModel<string>> RegisterAsync(UsuarioRegisterDto request);
    }
}
