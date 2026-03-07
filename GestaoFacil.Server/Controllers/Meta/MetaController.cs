using GestaoFacil.Server.DTOs.Meta;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Meta
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MetaController : BaseController
    {
        private readonly IMetaService _metaService;

        public MetaController(IMetaService metaService)
        {
            _metaService = metaService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<MetaDto>>>> GetAll()
        {
            var result = await _metaService.GetByUsuarioAsync(UsuarioId);
            return Ok(result);
        }

        [HttpGet("ativas")]
        public async Task<ActionResult<ResponseModel<List<MetaDto>>>> GetAtivas()
        {
            var result = await _metaService.GetAtivasAsync(UsuarioId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<MetaDto>>> GetById(int id)
        {
            var result = await _metaService.GetByIdAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<MetaDto>>> Create(MetaCreateDto dto)
        {
            var result = await _metaService.CreateAsync(dto, UsuarioId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Dados?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Update(int id, MetaUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ResponseHelper.Falha<bool>("O ID da rota não bate com o ID da meta."));
            }

            var result = await _metaService.UpdateAsync(id, dto, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> Delete(int id)
        {
            var result = await _metaService.DeleteAsync(id, UsuarioId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
