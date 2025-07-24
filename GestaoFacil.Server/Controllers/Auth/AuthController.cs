using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Shared.DTOs.Auth;
using GestaoFacil.Server.Services.Auth;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<TokenDto>>> Login(UsuarioLoginDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Status)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel<string>>> Register(UsuarioRegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ResponseModel<TokenDto>>> Refresh(RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if (!result.Status)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ResponseModel<string>>> Logout(RefreshTokenRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto.RefreshToken);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
