using GestaoFacil.Server.Data;
using GestaoFacil.Server.Services;
using GestaoFacil.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        private async Task<int?> GetUsuarioIdAsync() //metodo auxiliar que descobre qual usuário está fazendo a requisição
        {
            var email = User.FindFirstValue(ClaimTypes.Name); //extrai o e-mail do usuário logado do token JWT.
            if (string.IsNullOrEmpty(email)) return null;

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email); //busca o usuario no banco
            return usuario?.Id;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceitaDto>>> GetAll() //Task = retorno assincrono,
                                                                          //ActionResult é que vai ter um status
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
        public async Task<ActionResult<ReceitaDto>> Create(ReceitaCreateDto dto)
        {
            var usuarioId = await GetUsuarioIdAsync();
            if (usuarioId == null) return Unauthorized();

            var result = await _receitaService.CreateAsync(dto, usuarioId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ReceitaUpdateDto dto) //IActionResult = quando só importa o status da operacao
        {
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
