using AutoMapper;
using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Services.Despesa;

namespace GestaoFacil.Server.Services.Financeiro
{
    public class DespesaService
        : BaseFinanceiroService<DespesaModel, DespesaDto, DespesaCreateDto, DespesaUpdateDto, DespesaFiltroDto>,
          IDespesaService
    {
        public DespesaService(IDespesaRepository repository, IMapper mapper, ILogger<DespesaService> logger)
            : base(repository, mapper, logger) { }

        protected override string EntityName => "Despesa";

        protected override int GetUpdateDtoId(DespesaUpdateDto dto) => dto.Id;
        protected override int GetEntityUsuarioId(DespesaModel entity) => entity.UsuarioId;
        protected override void SetEntityUsuarioId(DespesaModel entity, int usuarioId) => entity.UsuarioId = usuarioId;
        protected override int GetEntityId(DespesaModel entity) => entity.Id;

        protected override string GetDataFormatada(DespesaModel entity) => entity.Data.ToString("dd/MM/yyyy HH:mm");
        protected override string GetNome(DespesaModel entity) => entity.Nome;
        protected override string GetDescricao(DespesaModel entity) => entity.Descricao ?? "";
        protected override Func<DespesaModel, string> GetCategoriaNome => d => d.CategoriaDespesa?.Nome ?? "";
        protected override string GetFormaPagamentoNome(DespesaModel entity) => entity.FormaPagamento?.Nome ?? "";
        protected override decimal GetValor(DespesaModel entity) => entity.Valor;
    }
}
