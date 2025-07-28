using AutoMapper;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.DTOs.Financeiro;

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

        public async Task<ResponseModel<List<ReceitaDto>>> GetRecentByUsuarioAsync(int usuarioId)
        {
            var receitas = await _repository.GetRecentByUsuarioAsync(usuarioId);
            var dtos = _mapper.Map<List<ReceitaDto>>(receitas);
            return ResponseHelper.Sucesso(dtos, "Receitas carregadas com sucesso.");
        }

        public async Task<ResponseModel<ReceitaDto?>> GetByIdAsync(int id, int usuarioId)
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

        public async Task<ResponseModel<ReceitaDto>> CreateAsync(ReceitaCreateDto dto, int usuarioId)
        {
            var receita = _mapper.Map<ReceitaModel>(dto);
            receita.UsuarioId = usuarioId;

            var criada = await _repository.AddAsync(receita);

            var dtoResult = _mapper.Map<ReceitaDto>(criada);

            _logger.LogInformation("Receita {Id} criada com sucesso para o usuário {UsuarioId}", criada.Id, usuarioId);
            return ResponseHelper.Sucesso(dtoResult, "Receita criada com sucesso.");
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId)
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

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
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
    }
}
