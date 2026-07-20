using GestaoFacil.Server.Controllers;
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
    public class CategoriaReceitaController : BaseController
    {
        private readonly AppDbContext _context;

        public CategoriaReceitaController(AppDbContext context)
        {
            _context = context;
        }

        //categorias de sistema (UsuarioId null) + as do proprio usuario
        private IQueryable<CategoriaReceitaModel> Visiveis() =>
            _context.CategoriasReceita.Where(c => c.UsuarioId == null || c.UsuarioId == UsuarioId);

        //somente as proprias podem ser alteradas
        private IQueryable<CategoriaReceitaModel> Editaveis() =>
            _context.CategoriasReceita.Where(c => c.UsuarioId == UsuarioId);

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<CategoriaDto>>>> GetAll()
        {
            var list = await Visiveis()
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var dto = list.Select(c => new CategoriaDto { Id = c.Id, Nome = c.Nome, Ativo = c.Ativo }).ToList();
            return Ok(ResponseHelper.Sucesso(dto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<CategoriaDto>>> GetById(int id)
        {
            var entity = await Visiveis().AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
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

            var nome = dto.Nome.Trim();

            var exists = await Visiveis().AnyAsync(c => c.Nome.ToLower() == nome.ToLower());
            if (exists)
                return BadRequest(ResponseHelper.Falha<CategoriaDto>("Já existe uma categoria com esse nome."));

            var entity = new CategoriaReceitaModel { Nome = nome, Ativo = true, UsuarioId = UsuarioId };
            _context.CategoriasReceita.Add(entity);
            await _context.SaveChangesAsync();

            var resDto = new CategoriaDto { Id = entity.Id, Nome = entity.Nome, Ativo = entity.Ativo };
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ResponseHelper.Sucesso(resDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, CategoriaUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ResponseHelper.Falha<bool>("ID inválido."));

            var entity = await Editaveis().FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
                return NotFound(ResponseHelper.Falha<bool>("Categoria não encontrada."));

            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(ResponseHelper.Falha<bool>("Nome é obrigatório."));

            var nome = dto.Nome.Trim();

            var exists = await Visiveis().AnyAsync(c => c.Id != id && c.Nome.ToLower() == nome.ToLower());
            if (exists)
                return BadRequest(ResponseHelper.Falha<bool>("Já existe uma categoria com esse nome."));

            entity.Nome = nome;
            entity.Ativo = dto.Ativo;

            await _context.SaveChangesAsync();

            return Ok(ResponseHelper.Sucesso(true));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var entity = await Editaveis().FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
                return NotFound(ResponseHelper.Falha<bool>("Categoria não encontrada."));

            // Marca como inativa para preservar histórico
            entity.Ativo = false;
            await _context.SaveChangesAsync();

            return Ok(ResponseHelper.Sucesso(true));
        }
    }
}
