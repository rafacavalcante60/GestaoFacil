using GestaoFacil.Server.DTOs.Relatorio;
using GestaoFacil.Server.Services.Relatorio;
using GestaoFacil.Server.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.Server.Controllers.Financeiro
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : BaseController
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("resumo")]
        public async Task<ActionResult<ResponseModel<ResumoFinanceiroDto>>> ResumoFinanceiro([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var result = await _relatorioService.ObterResumoFinanceiroAsync(UsuarioId, inicio, fim);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("categoria")]
        public async Task<ActionResult<ResponseModel<List<CategoriaResumoDto>>>> ResumoPorCategoria([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, [FromQuery] bool despesas = true)
        {
            var result = await _relatorioService.ObterResumoPorCategoriaAsync(UsuarioId, inicio, fim, despesas);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("fluxo")]
        public async Task<ActionResult<ResponseModel<List<FluxoCaixaDto>>>> FluxoCaixa([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var result = await _relatorioService.ObterFluxoCaixaAsync(UsuarioId, inicio, fim);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("mensal")]
        public async Task<ActionResult<ResponseModel<List<ResumoMensalDto>>>> ResumoMensal([FromQuery] int ano)
        {
            var result = await _relatorioService.ObterResumoMensalAsync(UsuarioId, ano);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
