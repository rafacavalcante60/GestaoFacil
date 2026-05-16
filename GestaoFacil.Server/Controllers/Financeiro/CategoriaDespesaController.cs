using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaDespesaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaDespesaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<CategoriaDto>>>> GetAll()
        {
            var list = await _context.CategoriasDespesa
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var dto = list.Select(c => new CategoriaDto { Id = c.Id, Nome = c.Nome, Ativo = c.Ativo }).ToList();
            return Ok(ResponseHelper.Sucesso(dto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<CategoriaDto>>> GetById(int id)
        {
            var entity = await _context.CategoriasDespesa.FindAsync(id);
            if (entity == null)
                return NotFound(ResponseHelper.Falha<CategoriaDto>("Categoria não encontrada."));

            var dto = new CategoriaDto { Id = entity.Id, Nome = entity.Nome, Ativo = entity.Ativo };
            return Ok(ResponseHelper.Sucesso(dto));
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<CategoriaDto>>> Create(CategoriaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(ResponseHelper.Falha<CategoriaDto>("Nome é obrigatório."));

            var exists = await _context.CategoriasDespesa.AnyAsync(c => c.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                return BadRequest(ResponseHelper.Falha<CategoriaDto>("Já existe uma categoria com esse nome."));

            var entity = new CategoriaDespesaModel { Nome = dto.Nome.Trim(), Ativo = true };
            _context.CategoriasDespesa.Add(entity);
            await _context.SaveChangesAsync();

            var resDto = new CategoriaDto { Id = entity.Id, Nome = entity.Nome, Ativo = entity.Ativo };
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ResponseHelper.Sucesso(resDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, CategoriaUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ResponseHelper.Falha<bool>("ID inválido."));

            var entity = await _context.CategoriasDespesa.FindAsync(id);
            if (entity == null)
                return NotFound(ResponseHelper.Falha<bool>("Categoria não encontrada."));

            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(ResponseHelper.Falha<bool>("Nome é obrigatório."));

            var exists = await _context.CategoriasDespesa.AnyAsync(c => c.Id != id && c.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                return BadRequest(ResponseHelper.Falha<bool>("Já existe uma categoria com esse nome."));

            entity.Nome = dto.Nome.Trim();
            entity.Ativo = dto.Ativo;

            _context.CategoriasDespesa.Update(entity);
            await _context.SaveChangesAsync();

            return Ok(ResponseHelper.Sucesso(true));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var entity = await _context.CategoriasDespesa.FindAsync(id);
            if (entity == null)
                return NotFound(ResponseHelper.Falha<bool>("Categoria não encontrada."));

            // Marca como inativa para preservar histórico
            entity.Ativo = false;
            _context.CategoriasDespesa.Update(entity);
            await _context.SaveChangesAsync();

            return Ok(ResponseHelper.Sucesso(true));
        }
    }
}
