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
        public async Task<ActionResult<ResponseModel<ResumoFinanceiroDto>>> ResumoFinanceiro([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, [FromQuery] int? categoriaDespesaId, [FromQuery] int? categoriaReceitaId, [FromQuery] int? formaPagamentoId)
        {
            var result = await _relatorioService.ObterResumoFinanceiroAsync(UsuarioId, inicio, fim, categoriaDespesaId, categoriaReceitaId, formaPagamentoId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("categoria")]
        public async Task<ActionResult<ResponseModel<List<CategoriaResumoDto>>>> ResumoPorCategoria([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, [FromQuery] bool despesas = true, [FromQuery] int? categoriaId = null, [FromQuery] int? formaPagamentoId = null)
        {
            var result = await _relatorioService.ObterResumoPorCategoriaAsync(UsuarioId, inicio, fim, despesas, categoriaId, formaPagamentoId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("fluxo")]
        public async Task<ActionResult<ResponseModel<List<FluxoCaixaDto>>>> FluxoCaixa([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, [FromQuery] int? categoriaDespesaId, [FromQuery] int? categoriaReceitaId, [FromQuery] int? formaPagamentoId)
        {
            var result = await _relatorioService.ObterFluxoCaixaAsync(UsuarioId, inicio, fim, categoriaDespesaId, categoriaReceitaId, formaPagamentoId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("mensal")]
        public async Task<ActionResult<ResponseModel<List<ResumoMensalDto>>>> ResumoMensal([FromQuery] int ano, [FromQuery] int? categoriaDespesaId, [FromQuery] int? categoriaReceitaId, [FromQuery] int? formaPagamentoId)
        {
            var result = await _relatorioService.ObterResumoMensalAsync(UsuarioId, ano, categoriaDespesaId, categoriaReceitaId, formaPagamentoId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
