using GestaoFacil.Server.Services;
using GestaoFacil.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GestaoFacil.Server.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GestaoFacil.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitaController : ControllerBase
    {
        private readonly IReceitaService _receitaService;
        private readonly AppDbContext _context;

        public ReceitaController(IReceitaService receitaService, AppDbContext context)
        {
            _receitaService = receitaService;
            _context = context;
        }

        private async Task<int?> GetUsuarioIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(email)) return null;

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            return usuario?.Id;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceitaDto>>> GetAll()
        {
            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var result = await _receitaService.GetAllByUsuarioAsync(usuarioId.Value);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceitaDto>> GetById(int id)
        {
            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var result = await _receitaService.GetByIdAsync(id, usuarioId.Value);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ReceitaDto>> Create([FromBody] ReceitaCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var result = await _receitaService.CreateAsync(dto, usuarioId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReceitaUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest("ID do recurso diferente do corpo da requisição.");

            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var success = await _receitaService.UpdateAsync(id, dto, usuarioId.Value);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var success = await _receitaService.DeleteAsync(id, usuarioId.Value);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
