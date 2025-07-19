using GestaoFacil.Shared.Responses;
using GestaoFacil.Server.Services.Despesa;
using GestaoFacil.Shared.DTOs.Despesa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoFacil.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DespesaController : ControllerBase
    {
        private readonly IDespesaService _despesaService;

        public DespesaController(IDespesaService despesaService)
        {
            _despesaService = despesaService;
        }

        // obtem id por meio de claims
        private int GetUsuarioId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : throw new UnauthorizedAccessException("Usuário inválido.");
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<DespesaDto>>>> GetAll()
        {
            var usuarioId = GetUsuarioId();

            var result = await _despesaService.GetAllByUsuarioAsync(usuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<DespesaDto>>> GetById(int id)
        {
            var usuarioId = GetUsuarioId();

            var result = await _despesaService.GetByIdAsync(id, usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<DespesaDto>>> Create([FromBody] DespesaCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = GetUsuarioId();

            var result = await _despesaService.CreateAsync(dto, usuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Dados?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, [FromBody] DespesaUpdateDto dto)
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

            var result = await _despesaService.UpdateAsync(id, dto, usuarioId);

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

            var result = await _despesaService.DeleteAsync(id, usuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
