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
        public async Task<ActionResult<ResponseModel<PagedList<DespesaDto>>>> GetByUsuarioPaged([FromQuery] QueryStringParameters parameters)
        {
            var result = await _despesaService.GetByUsuarioPagedAsync(UsuarioId, parameters);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return ObterDespesas(result);
        }

        [HttpPost("filter/pagination")]
        public async Task<ActionResult<ResponseModel<PagedList<DespesaDto>>>> FiltrarPaged([FromQuery] DespesaFiltroDto filtro)
        {
            var result = await _despesaService.FiltrarPagedAsync(UsuarioId, filtro);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return ObterDespesas(result);
        }

        private ActionResult<ResponseModel<PagedList<DespesaDto>>> ObterDespesas(ResponseModel<PagedList<DespesaDto>> result)
        {
            var metadata = new
            {
                result.Dados!.CurrentPage,
                result.Dados!.TotalPages,
                result.Dados!.PageSize,
                result.Dados!.TotalCount,
                result.Dados!.HasNext,
                result.Dados!.HasPrevious
            };

            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata)); //adiciona os metadados de paginação no cabeçalho da resposta

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
    }
}
