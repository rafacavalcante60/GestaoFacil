using GestaoFacil.Shared.Responses;
using GestaoFacil.Server.Services.Despesa;
using GestaoFacil.Shared.DTOs.Despesa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    //sem uso de controller generico (com receita) visando clareza e escalabilidade
    //[Authorize]
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

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<DespesaDto>>>> GetRecentByUsuario()
        {
            var result = await _despesaService.GetRecentByUsuarioAsync(UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

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
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id,DespesaUpdateDto dto)
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
