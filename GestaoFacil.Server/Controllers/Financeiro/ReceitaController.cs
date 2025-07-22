using GestaoFacil.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Server.Services.Financeiro;
using GestaoFacil.Shared.DTOs.Financeiro;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    //sem uso de controller generico (com despesa) visando clareza e escalabilidade
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

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReceitaDto>>>> GetAll()
        {
            var result = await _receitaService.GetAllByUsuarioAsync(UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
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
    }
}
