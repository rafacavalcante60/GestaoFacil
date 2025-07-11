using GestaoFacil.Shared.DTOs.Auth;

namespace GestaoFacil.Server.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> RegisterAsync(RegisterRequest request);
    }
}
