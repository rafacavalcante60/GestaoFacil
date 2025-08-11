using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Financeiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    //sem uso de controller generico (com despesa) visando clareza e flexibilidade para as 2 entidades
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitaController : BaseController
    {
        private readonly IReceitaService _receitaService;

        public ReceitaController(IReceitaService receitaService)
        {
            _receitaService = receitaService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<ReceitaDto>>> GetById(int id)
        {
            var result = await _receitaService.GetByIdAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<ResponseModel<PagedList<ReceitaDto>>>> GetByUsuarioPaged([FromQuery] QueryStringParameters parameters)
        {
            var result = await _receitaService.GetByUsuarioPagedAsync(UsuarioId, parameters);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return ObterReceitas(result);
        }

        [HttpPost("filter/pagination")]
        public async Task<ActionResult<ResponseModel<PagedList<ReceitaDto>>>> FiltrarPaged([FromQuery] ReceitaFiltroDto filtro)
        {
            var result = await _receitaService.FiltrarPagedAsync(UsuarioId, filtro);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return ObterReceitas(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<ReceitaDto>>> Create(ReceitaCreateDto dto)
        {
            var result = await _receitaService.CreateAsync(dto, UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Dados?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, ReceitaUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ResponseHelper.Falha<bool>("O ID da rota não corresponde ao ID da receita."));
            }

            var result = await _receitaService.UpdateAsync(id, dto, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var result = await _receitaService.DeleteAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        private ActionResult<ResponseModel<PagedList<ReceitaDto>>> ObterReceitas(ResponseModel<PagedList<ReceitaDto>> result)
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
    }
}
