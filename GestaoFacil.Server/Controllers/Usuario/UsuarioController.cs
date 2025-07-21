using GestaoFacil.Shared.Responses;
using GestaoFacil.Shared.DTOs.Usuario;
using GestaoFacil.Server.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Usuario
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("perfil")]
        public async Task<ActionResult<ResponseModel<UsuarioDto>>> GetPerfil()
        {
            var result = await _usuarioService.GetByIdAsync(UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("perfil")]
        public async Task<ActionResult<ResponseModel<bool>>> UpdatePerfil([FromBody] UsuarioUpdateDto dto)
        {
            var result = await _usuarioService.UpdatePerfilAsync(UsuarioId, dto);

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
            if (id != dto.Id)
            {
                return BadRequest(ResponseHelper.Falha<bool>("O ID da URL não corresponde ao ID do corpo."));
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
