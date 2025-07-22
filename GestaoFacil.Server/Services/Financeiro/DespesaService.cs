using AutoMapper;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.Responses;
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

        public async Task<ResponseModel<List<DespesaDto>>> GetAllByUsuarioAsync(int usuarioId)
        {
            try
            {
                var despesas = await _repository.GetAllByUsuarioAsync(usuarioId);
                var dtos = _mapper.Map<List<DespesaDto>>(despesas);
                return ResponseHelper.Sucesso(dtos, "Despesas carregadas com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar despesas para o usuário {UsuarioId}", usuarioId);
                return ResponseHelper.Falha<List<DespesaDto>>($"Erro ao buscar despesas: {ex.Message}");
            }
        }

        public async Task<ResponseModel<DespesaDto?>> GetByIdAsync(int id, int usuarioId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar despesa {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<DespesaDto?>($"Erro ao buscar despesa: {ex.Message}");
            }
        }

        public async Task<ResponseModel<DespesaDto>> CreateAsync(DespesaCreateDto dto, int usuarioId)
        {
            try
            {
                var despesa = _mapper.Map<DespesaModel>(dto);
                despesa.UsuarioId = usuarioId;

                await _repository.AddAsync(despesa);
                var criada = await _repository.GetByIdAsync(despesa.Id, usuarioId);

                var dtoResult = _mapper.Map<DespesaDto>(criada);

                _logger.LogInformation("Despesa criada com ID {Id} para o usuário {UsuarioId}", despesa.Id, usuarioId);
                return ResponseHelper.Sucesso(dtoResult, "Despesa criada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar despesa para o usuário {UsuarioId}", usuarioId);
                return ResponseHelper.Falha<DespesaDto>($"Erro ao criar despesa: {ex.Message}");
            }
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar despesa {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>($"Erro ao atualizar despesa: {ex.Message}", false);
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover despesa {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>($"Erro ao remover despesa: {ex.Message}", false);
            }
        }
    }
}
