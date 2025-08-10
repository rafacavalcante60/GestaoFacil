using AutoMapper;
using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Filtro;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Responses;
using Microsoft.Extensions.Logging;

namespace GestaoFacil.Server.Services.Despesa
{
    public class DespesaService : IDespesaService
    {
        private readonly IDespesaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DespesaService> _logger;

        public DespesaService(IDespesaRepository repository, IMapper mapper, ILogger<DespesaService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseModel<DespesaDto?>> GetByIdAsync(int id, int usuarioId)
        {
            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            if (despesa == null)
            {
                _logger.LogWarning("Despesa {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<DespesaDto?>("Despesa não encontrada.");
            }

            var dto = _mapper.Map<DespesaDto>(despesa);
            return ResponseHelper.Sucesso<DespesaDto?>(dto, "Despesa localizada com sucesso.");
        }

        public async Task<ResponseModel<PagedList<DespesaDto>>> GetByUsuarioPagedAsync(int usuarioId, Parameters parameters)
        {
            var despesas = await _repository.GetByUsuarioIdPagedAsync(usuarioId, parameters.PageNumber, parameters.PageSize);

            var dtos = new PagedList<DespesaDto>(
                _mapper.Map<List<DespesaDto>>(despesas),
                despesas.TotalCount,
                despesas.CurrentPage,
                despesas.PageSize
            );

            return ResponseHelper.Sucesso(dtos, "Despesas paginadas carregadas com sucesso.");
        }



        public async Task<ResponseModel<DespesaDto>> CreateAsync(DespesaCreateDto dto, int usuarioId)
        {
            var despesa = _mapper.Map<DespesaModel>(dto);
            despesa.UsuarioId = usuarioId;

            var criada = await _repository.AddAsync(despesa);

            var dtoResult = _mapper.Map<DespesaDto>(criada);

            _logger.LogInformation("Despesa criada com ID {Id} para o usuário {UsuarioId}", criada.Id, usuarioId);
            return ResponseHelper.Sucesso(dtoResult, "Despesa criada com sucesso.");
        }


        public async Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID do corpo ({BodyId}) não bate com ID da rota ({RouteId})", dto.Id, id);
                return ResponseHelper.Falha<bool>("ID da despesa inválido.");
            }

            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            if (despesa == null)
            {
                _logger.LogWarning("Despesa {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>("Despesa não encontrada.");
            }

            _mapper.Map(dto, despesa);
            await _repository.UpdateAsync(despesa);

            _logger.LogInformation("Despesa {Id} atualizada para o usuário {UsuarioId}", id, usuarioId);
            return ResponseHelper.Sucesso(true, "Despesa atualizada com sucesso.");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            if (despesa == null)
            {
                _logger.LogWarning("Tentativa de remover despesa {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>("Despesa não encontrada.");
            }

            await _repository.DeleteAsync(despesa);

            _logger.LogInformation("Despesa {Id} removida para o usuário {UsuarioId}", id, usuarioId);
            return ResponseHelper.Sucesso(true, "Despesa removida com sucesso.");
        }

        public async Task<ResponseModel<List<DespesaDto>>> FiltrarAsync(DespesaFiltroDto filtro, int usuarioId)
        {
            if (filtro.DataInicial.HasValue && filtro.DataFinal.HasValue && filtro.DataInicial > filtro.DataFinal)
            {
                _logger.LogWarning("Filtro inválido: DataInicial {DataInicial} maior que DataFinal {DataFinal} para usuário {UsuarioId}",
                    filtro.DataInicial, filtro.DataFinal, usuarioId);

                return ResponseHelper.Falha<List<DespesaDto>>("A data inicial não pode ser maior que a data final.");
            }

            var despesas = await _repository.FiltrarAsync(filtro, usuarioId);
            var dto = _mapper.Map<List<DespesaDto>>(despesas);

            return ResponseHelper.Sucesso(dto);
        }
    }
}
