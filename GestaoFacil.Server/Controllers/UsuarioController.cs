using GestaoFacil.Shared.Responses;
using GestaoFacil.Shared.DTOs.Usuario;
using GestaoFacil.Server.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoFacil.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        private int GetUsuarioId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuário inválido.");
        }

        [HttpGet("perfil")]
        public async Task<ActionResult<ResponseModel<UsuarioDto>>> GetPerfil()
        {
            var usuarioId = GetUsuarioId();

            var result = await _usuarioService.GetByIdAsync(usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("perfil")]
        public async Task<ActionResult<ResponseModel<bool>>> UpdatePerfil([FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = GetUsuarioId();

            var result = await _usuarioService.UpdatePerfilAsync(usuarioId, dto);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<UsuarioDto>>>> GetAll()
        {
            var result = await _usuarioService.GetAllAsync();

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> UpdateAdmin(int id, [FromBody] UsuarioAdminUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var result = await _usuarioService.UpdateAdminAsync(id, dto);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var result = await _usuarioService.DeleteAsync(id);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
