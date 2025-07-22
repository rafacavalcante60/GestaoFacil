using AutoMapper;
using GestaoFacil.Shared.Responses;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Shared.DTOs.Financeiro;
using Microsoft.Extensions.Logging;

namespace GestaoFacil.Server.Services.Financeiro
{
    public class ReceitaService : IReceitaService
    {
        private readonly IReceitaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReceitaService> _logger;

        public ReceitaService(IReceitaRepository repository, IMapper mapper, ILogger<ReceitaService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseModel<List<ReceitaDto>>> GetAllByUsuarioAsync(int usuarioId)
        {
            try
            {
                var receitas = await _repository.GetAllByUsuarioAsync(usuarioId);
                var dtos = _mapper.Map<List<ReceitaDto>>(receitas);
                return ResponseHelper.Sucesso(dtos, "Receitas carregadas com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar receitas para o usuário {UsuarioId}", usuarioId);
                return ResponseHelper.Falha<List<ReceitaDto>>($"Erro ao buscar receitas: {ex.Message}");
            }
        }

        public async Task<ResponseModel<ReceitaDto?>> GetByIdAsync(int id, int usuarioId)
        {
            try
            {
                var receita = await _repository.GetByIdAsync(id, usuarioId);
                if (receita == null)
                {
                    _logger.LogWarning("Receita {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                    return ResponseHelper.Falha<ReceitaDto?>("Receita não encontrada.");
                }

                var dto = _mapper.Map<ReceitaDto>(receita);
                return ResponseHelper.Sucesso<ReceitaDto?>(dto, "Receita localizada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar receita {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<ReceitaDto?>($"Erro ao buscar receita: {ex.Message}");
            }
        }

        public async Task<ResponseModel<ReceitaDto>> CreateAsync(ReceitaCreateDto dto, int usuarioId)
        {
            try
            {
                var receita = _mapper.Map<ReceitaModel>(dto);
                receita.UsuarioId = usuarioId;

                await _repository.AddAsync(receita);

                var criada = await _repository.GetByIdAsync(receita.Id, usuarioId);
                var dtoResult = _mapper.Map<ReceitaDto>(criada);

                _logger.LogInformation("Receita {Id} criada com sucesso para o usuário {UsuarioId}", receita.Id, usuarioId);
                return ResponseHelper.Sucesso(dtoResult, "Receita criada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar receita para o usuário {UsuarioId}", usuarioId);
                return ResponseHelper.Falha<ReceitaDto>($"Erro ao criar receita: {ex.Message}");
            }
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId)
        {
            try
            {
                if (id != dto.Id)
                {
                    _logger.LogWarning("ID da rota ({RouteId}) não bate com o corpo ({BodyId}) para receita", id, dto.Id);
                    return ResponseHelper.Falha<bool>("ID da receita inválido.");
                }

                var receita = await _repository.GetByIdAsync(id, usuarioId);
                if (receita == null)
                {
                    _logger.LogWarning("Receita {Id} não encontrada para atualização do usuário {UsuarioId}", id, usuarioId);
                    return ResponseHelper.Falha<bool>("Receita não encontrada.");
                }

                _mapper.Map(dto, receita);
                await _repository.UpdateAsync(receita);

                _logger.LogInformation("Receita {Id} atualizada com sucesso para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Sucesso(true, "Receita atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar receita {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha($"Erro ao atualizar receita: {ex.Message}", false);
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            try
            {
                var receita = await _repository.GetByIdAsync(id, usuarioId);
                if (receita == null)
                {
                    _logger.LogWarning("Receita {Id} não encontrada para remoção do usuário {UsuarioId}", id, usuarioId);
                    return ResponseHelper.Falha<bool>("Receita não encontrada.");
                }

                await _repository.DeleteAsync(receita);

                _logger.LogInformation("Receita {Id} removida com sucesso para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Sucesso(true, "Receita removida com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover receita {Id} para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha($"Erro ao remover receita: {ex.Message}", false);
            }
        }
    }
}
