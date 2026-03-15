using AutoMapper;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Financeiro
{
    public abstract class BaseFinanceiroService<TModel, TDto, TCreateDto, TUpdateDto, TFiltro>
        where TModel : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
        where TFiltro : QueryStringParameters, IFiltroData
    {
        protected readonly IFinanceiroRepository<TModel, TFiltro> Repository;
        protected readonly IMapper Mapper;
        protected readonly ILogger Logger;

        protected abstract string EntityName { get; }
        protected abstract int GetUpdateDtoId(TUpdateDto dto);
        protected abstract int GetEntityUsuarioId(TModel entity);
        protected abstract void SetEntityUsuarioId(TModel entity, int usuarioId);
        protected abstract int GetEntityId(TModel entity);
        protected abstract Func<TModel, string> GetCategoriaNome { get; }

        protected BaseFinanceiroService(IFinanceiroRepository<TModel, TFiltro> repository, IMapper mapper, ILogger logger)
        {
            Repository = repository;
            Mapper = mapper;
            Logger = logger;
        }

        public async Task<ResponseModel<TDto?>> GetByIdAsync(int id, int usuarioId)
        {
            var entity = await Repository.GetByIdAsync(id, usuarioId);
            if (entity == null)
            {
                Logger.LogWarning("{Entity} {Id} não encontrada para o usuário {UsuarioId}", EntityName, id, usuarioId);
                return ResponseHelper.Falha<TDto?>($"{EntityName} não encontrada.");
            }

            var dto = Mapper.Map<TDto>(entity);
            return ResponseHelper.Sucesso<TDto?>(dto, $"{EntityName} localizada com sucesso.");
        }

        public async Task<ResponseModel<PagedList<TDto>>> GetByUsuarioPagedAsync(int usuarioId, QueryStringParameters parameters)
        {
            var entities = await Repository.GetByUsuarioIdPagedAsync(usuarioId, parameters.PageNumber, parameters.PageSize);

            var dtos = new PagedList<TDto>(
                Mapper.Map<List<TDto>>(entities),
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );

            return ResponseHelper.Sucesso(dtos, $"{EntityName}s paginadas carregadas com sucesso.");
        }

        public async Task<ResponseModel<PagedList<TDto>>> FiltrarPagedAsync(int usuarioId, TFiltro filtro)
        {
            var erroData = FinanceiroHelper.ValidarFiltroData<PagedList<TDto>>(filtro.DataInicial, filtro.DataFinal, Logger, usuarioId);
            if (erroData != null) return erroData;

            var entities = await Repository.FiltrarPagedAsync(usuarioId, filtro);

            var dtos = new PagedList<TDto>(
                Mapper.Map<List<TDto>>(entities),
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );

            return ResponseHelper.Sucesso(dtos, $"{EntityName}s filtradas e paginadas carregadas com sucesso.");
        }

        public async Task<ResponseModel<byte[]>> ExportarExcelCompletoAsync(int usuarioId, TFiltro filtro)
        {
            var erroData = FinanceiroHelper.ValidarFiltroData<byte[]>(filtro.DataInicial, filtro.DataFinal, Logger, usuarioId);
            if (erroData != null) return erroData;

            var entities = await Repository.FiltrarAsync(usuarioId, filtro);

            if (!entities.Any())
            {
                return ResponseHelper.Falha<byte[]>($"Nenhuma {EntityName.ToLower()} encontrada para exportação.");
            }

            var bytes = FinanceiroHelper.GerarExcel(
                entities,
                $"{EntityName}s",
                GetDataFormatada,
                GetNome,
                GetDescricao,
                GetCategoriaNome,
                GetFormaPagamentoNome,
                GetValor
            );

            return ResponseHelper.Sucesso(bytes, $"Relatório de {EntityName.ToLower()}s exportado com sucesso.");
        }

        protected abstract string GetDataFormatada(TModel entity);
        protected abstract string GetNome(TModel entity);
        protected abstract string GetDescricao(TModel entity);
        protected abstract string GetFormaPagamentoNome(TModel entity);
        protected abstract decimal GetValor(TModel entity);

        public async Task<ResponseModel<TDto>> CreateAsync(TCreateDto dto, int usuarioId)
        {
            var entity = Mapper.Map<TModel>(dto);
            SetEntityUsuarioId(entity, usuarioId);

            var criada = await Repository.AddAsync(entity);

            var dtoResult = Mapper.Map<TDto>(criada);

            Logger.LogInformation("{Entity} criada com ID {Id} para o usuário {UsuarioId}", EntityName, GetEntityId(criada), usuarioId);
            return ResponseHelper.Sucesso(dtoResult, $"{EntityName} criada com sucesso.");
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, TUpdateDto dto, int usuarioId)
        {
            if (id != GetUpdateDtoId(dto))
            {
                Logger.LogWarning("ID do corpo não bate com ID da rota para {Entity}", EntityName);
                return ResponseHelper.Falha<bool>($"ID da {EntityName.ToLower()} inválido.");
            }

            var entity = await Repository.GetByIdAsync(id, usuarioId);
            if (entity == null)
            {
                Logger.LogWarning("{Entity} {Id} não encontrada para o usuário {UsuarioId}", EntityName, id, usuarioId);
                return ResponseHelper.Falha<bool>($"{EntityName} não encontrada.");
            }

            Mapper.Map(dto, entity);
            await Repository.UpdateAsync(entity);

            Logger.LogInformation("{Entity} {Id} atualizada para o usuário {UsuarioId}", EntityName, id, usuarioId);
            return ResponseHelper.Sucesso(true, $"{EntityName} atualizada com sucesso.");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            var entity = await Repository.GetByIdAsync(id, usuarioId);
            if (entity == null)
            {
                Logger.LogWarning("Tentativa de remover {Entity} {Id} não encontrada para o usuário {UsuarioId}", EntityName, id, usuarioId);
                return ResponseHelper.Falha<bool>($"{EntityName} não encontrada.");
            }

            await Repository.DeleteAsync(entity);

            Logger.LogInformation("{Entity} {Id} removida para o usuário {UsuarioId}", EntityName, id, usuarioId);
            return ResponseHelper.Sucesso(true, $"{EntityName} removida com sucesso.");
        }
    }
}
