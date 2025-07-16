using AutoMapper;
using GestaoFacil.Shared.Responses;
using GestaoFacil.Server.Repositories.Receita;
using GestaoFacil.Shared.DTOs.Receita;
using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Services.Receita
{
    public class ReceitaService : IReceitaService
    {
        private readonly IReceitaRepository _repository;
        private readonly IMapper _mapper;

        public ReceitaService(IReceitaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
                    return ResponseHelper.Falha<ReceitaDto?>("Receita não encontrada.");
                }

                var dto = _mapper.Map<ReceitaDto>(receita);
                return ResponseHelper.Sucesso<ReceitaDto?>(dto, "Receita localizada com sucesso.");
            }
            catch (Exception ex)
            {
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
                return ResponseHelper.Sucesso(dtoResult, "Receita criada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Falha<ReceitaDto>($"Erro ao criar receita: {ex.Message}");
            }
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId)
        {
            try
            {
                if (id != dto.Id)
                {
                    return ResponseHelper.Falha<bool>("ID da receita inválido.");
                }

                var receita = await _repository.GetByIdAsync(id, usuarioId);
                if (receita == null)
                {
                    return ResponseHelper.Falha<bool>("Receita não encontrada.");
                }

                _mapper.Map(dto, receita);
                await _repository.UpdateAsync(receita);

                return ResponseHelper.Sucesso(true, "Receita atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Falha<bool>($"Erro ao atualizar receita: {ex.Message}", false);
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            try
            {
                var receita = await _repository.GetByIdAsync(id, usuarioId);
                if (receita == null)
                {
                    return ResponseHelper.Falha<bool>("Receita não encontrada.");
                }

                await _repository.DeleteAsync(receita);
                return ResponseHelper.Sucesso(true, "Receita removida com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Falha<bool>($"Erro ao remover receita: {ex.Message}", false);
            }
        }
    }
}
