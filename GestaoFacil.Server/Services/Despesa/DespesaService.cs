using AutoMapper;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Services.Despesa
{
    public class DespesaService : IDespesaService
    {
        private readonly IDespesaRepository _repository;
        private readonly IMapper _mapper;

        public DespesaService(IDespesaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
                    return ResponseHelper.Falha<DespesaDto?>("Despesa não encontrada.");
                }

                var dto = _mapper.Map<DespesaDto>(despesa);
                return ResponseHelper.Sucesso<DespesaDto?>(dto, "Despesa localizada com sucesso.");
            }
            catch (Exception ex)
            {
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
                return ResponseHelper.Sucesso(dtoResult, "Despesa criada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Falha<DespesaDto>($"Erro ao criar despesa: {ex.Message}");
            }
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId)
        {
            try
            {
                if (id != dto.Id)
                {
                    return ResponseHelper.Falha<bool>("ID da despesa inválido.");
                }

                var despesa = await _repository.GetByIdAsync(id, usuarioId);
                if (despesa == null)
                {
                    return ResponseHelper.Falha<bool>("Despesa não encontrada.");
                }

                _mapper.Map(dto, despesa);
                await _repository.UpdateAsync(despesa);

                return ResponseHelper.Sucesso(true, "Despesa atualizada com sucesso.");
            }
            catch (Exception ex)
            {
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
                    return ResponseHelper.Falha<bool>("Despesa não encontrada.");
                }

                await _repository.DeleteAsync(despesa);
                return ResponseHelper.Sucesso(true, "Despesa removida com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Falha<bool>($"Erro ao remover despesa: {ex.Message}", false);
            }
        }
    }
}
