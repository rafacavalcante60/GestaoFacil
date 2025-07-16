using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Shared.DTOs.Auth;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GestaoFacil.Server.Services.Auth;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return result.StatusCode != 0
                    ? StatusCode(result.StatusCode, result.Message)
                    : BadRequest(result.Message);

            }
            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return result.StatusCode != 0
                    ? StatusCode(result.StatusCode, result.Message)
                    : BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
    }
}
