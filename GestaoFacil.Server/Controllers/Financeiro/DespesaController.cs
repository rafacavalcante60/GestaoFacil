using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Despesa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    //sem uso de controller generico (com receita) visando clareza e flexibilidade para as 2 entidades
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DespesaController : BaseController
    {
        private readonly IDespesaService _despesaService;

        public DespesaController(IDespesaService despesaService)
        {
            _despesaService = despesaService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<DespesaDto>>> GetById(int id)
        {
            var result = await _despesaService.GetByIdAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<ResponseModel<List<DespesaDto>>>> GetByUsuarioPaged([FromQuery] Parameters parameters) //objeto complexo = FromQuery
        {
            var result = await _despesaService.GetByUsuarioPagedAsync(UsuarioId, parameters);

            if (!result.Status)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<DespesaDto>>> Create(DespesaCreateDto dto)
        {
            var result = await _despesaService.CreateAsync(dto, UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Dados?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, DespesaUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ResponseHelper.Falha<bool>("O ID da rota não bate com o ID da despesa."));
            }

            var result = await _despesaService.UpdateAsync(id, dto, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var result = await _despesaService.DeleteAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("filtrar")]
        public async Task<ActionResult<ResponseModel<List<DespesaDto>>>> Filtrar(DespesaFiltroDto filtro)
        {
            var result = await _despesaService.FiltrarAsync(filtro, UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
