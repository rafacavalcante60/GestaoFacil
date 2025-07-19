using GestaoFacil.Shared.Responses;
using GestaoFacil.Server.Services.Receita;
using GestaoFacil.Shared.DTOs.Receita;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoFacil.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitaController : ControllerBase
    {
        private readonly IReceitaService _receitaService;

        public ReceitaController(IReceitaService receitaService)
        {
            _receitaService = receitaService;
        }

        private int GetUsuarioId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuário inválido.");
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReceitaDto>>>> GetAll()
        {
            var usuarioId = GetUsuarioId();

            var result = await _receitaService.GetAllByUsuarioAsync(usuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<ReceitaDto>>> GetById(int id)
        {
            var usuarioId = GetUsuarioId();

            var result = await _receitaService.GetByIdAsync(id, usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<ReceitaDto>>> Create([FromBody] ReceitaCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = GetUsuarioId();

            var result = await _receitaService.CreateAsync(dto, usuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Dados?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, [FromBody] ReceitaUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var usuarioId = GetUsuarioId();

            var result = await _receitaService.UpdateAsync(id, dto, usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var usuarioId = GetUsuarioId();

            var result = await _receitaService.DeleteAsync(id, usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
