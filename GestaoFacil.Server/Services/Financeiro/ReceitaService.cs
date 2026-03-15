using AutoMapper;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.DTOs.Financeiro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Financeiro;

namespace GestaoFacil.Server.Services.Financeiro
{
    public class ReceitaService
        : BaseFinanceiroService<ReceitaModel, ReceitaDto, ReceitaCreateDto, ReceitaUpdateDto, ReceitaFiltroDto>,
          IReceitaService
    {
        public ReceitaService(IReceitaRepository repository, IMapper mapper, ILogger<ReceitaService> logger)
            : base(repository, mapper, logger) { }

        protected override string EntityName => "Receita";

        protected override int GetUpdateDtoId(ReceitaUpdateDto dto) => dto.Id;
        protected override int GetEntityUsuarioId(ReceitaModel entity) => entity.UsuarioId;
        protected override void SetEntityUsuarioId(ReceitaModel entity, int usuarioId) => entity.UsuarioId = usuarioId;
        protected override int GetEntityId(ReceitaModel entity) => entity.Id;

        protected override string GetDataFormatada(ReceitaModel entity) => entity.Data.ToString("dd/MM/yyyy HH:mm");
        protected override string GetNome(ReceitaModel entity) => entity.Nome;
        protected override string GetDescricao(ReceitaModel entity) => entity.Descricao ?? "";
        protected override Func<ReceitaModel, string> GetCategoriaNome => r => r.CategoriaReceita?.Nome ?? "";
        protected override string GetFormaPagamentoNome(ReceitaModel entity) => entity.FormaPagamento?.Nome ?? "";
        protected override decimal GetValor(ReceitaModel entity) => entity.Valor;
    }
}
