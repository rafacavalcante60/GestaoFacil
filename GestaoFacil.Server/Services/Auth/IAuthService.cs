using GestaoFacil.Shared.DTOs.Auth;

namespace GestaoFacil.Server.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> RegisterAsync(RegisterRequest request);
    }
}
