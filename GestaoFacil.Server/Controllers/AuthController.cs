using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Shared.DTOs.Auth;
using GestaoFacil.Server.Services.Auth;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<TokenDto>>> Login([FromBody] UsuarioLoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseHelper.Falha<TokenDto>("Dados de login inválidos."));
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Status)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel<string>>> Register([FromBody] UsuarioRegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseHelper.Falha<string>("Dados de registro inválidos."));
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
