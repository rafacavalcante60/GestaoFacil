using AutoMapper;
using GestaoFacil.Server.DTOs.Meta;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Meta;
using GestaoFacil.Server.Responses;
using Microsoft.Extensions.Logging;

namespace GestaoFacil.Server.Services.Meta
{
    public class MetaService : IMetaService
    {
        private readonly IMetaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaService> _logger;

        public MetaService(IMetaRepository repository, IMapper mapper, ILogger<MetaService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseModel<List<MetaDto>>> GetByUsuarioAsync(int usuarioId)
        {
            var metas = await _repository.GetByUsuarioAsync(usuarioId);
            var dtos = new List<MetaDto>();

            foreach (var meta in metas)
            {
                dtos.Add(await CalcularProgressoAsync(meta));
            }

            return ResponseHelper.Sucesso(dtos, "Metas carregadas com sucesso.");
        }

        public async Task<ResponseModel<List<MetaDto>>> GetAtivasAsync(int usuarioId)
        {
            var metas = await _repository.GetAtivasAsync(usuarioId);
            var dtos = new List<MetaDto>();

            foreach (var meta in metas)
            {
                dtos.Add(await CalcularProgressoAsync(meta));
            }

            return ResponseHelper.Sucesso(dtos, "Metas ativas carregadas com sucesso.");
        }

        public async Task<ResponseModel<MetaDto?>> GetByIdAsync(int id, int usuarioId)
        {
            var meta = await _repository.GetByIdAsync(id, usuarioId);
            if (meta == null)
            {
                _logger.LogWarning("Meta {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<MetaDto?>("Meta não encontrada.");
            }

            var dto = await CalcularProgressoAsync(meta);
            return ResponseHelper.Sucesso<MetaDto?>(dto, "Meta localizada com sucesso.");
        }

        public async Task<ResponseModel<MetaDto>> CreateAsync(MetaCreateDto dto, int usuarioId)
        {
            if (dto.ValorMeta <= 0)
            {
                return ResponseHelper.Falha<MetaDto>("O valor da meta deve ser maior que zero.");
            }

            if (dto.DataInicio > dto.DataFim)
            {
                return ResponseHelper.Falha<MetaDto>("A data de início não pode ser maior que a data de fim.");
            }

            var meta = _mapper.Map<MetaFinanceiraModel>(dto);
            meta.UsuarioId = usuarioId;

            var criada = await _repository.AddAsync(meta);
            var dtoResult = await CalcularProgressoAsync(criada);

            _logger.LogInformation("Meta criada com ID {Id} para o usuário {UsuarioId}", criada.Id, usuarioId);
            return ResponseHelper.Sucesso(dtoResult, "Meta criada com sucesso.");
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, MetaUpdateDto dto, int usuarioId)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID do corpo ({BodyId}) não bate com ID da rota ({RouteId})", dto.Id, id);
                return ResponseHelper.Falha<bool>("ID da meta inválido.");
            }

            if (dto.ValorMeta <= 0)
            {
                return ResponseHelper.Falha<bool>("O valor da meta deve ser maior que zero.");
            }

            if (dto.DataInicio > dto.DataFim)
            {
                return ResponseHelper.Falha<bool>("A data de início não pode ser maior que a data de fim.");
            }

            var meta = await _repository.GetByIdAsync(id, usuarioId);
            if (meta == null)
            {
                _logger.LogWarning("Meta {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>("Meta não encontrada.");
            }

            _mapper.Map(dto, meta);
            await _repository.UpdateAsync(meta);

            _logger.LogInformation("Meta {Id} atualizada para o usuário {UsuarioId}", id, usuarioId);
            return ResponseHelper.Sucesso(true, "Meta atualizada com sucesso.");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId)
        {
            var meta = await _repository.GetByIdAsync(id, usuarioId);
            if (meta == null)
            {
                _logger.LogWarning("Tentativa de remover meta {Id} não encontrada para o usuário {UsuarioId}", id, usuarioId);
                return ResponseHelper.Falha<bool>("Meta não encontrada.");
            }

            await _repository.DeleteAsync(meta);

            _logger.LogInformation("Meta {Id} removida para o usuário {UsuarioId}", id, usuarioId);
            return ResponseHelper.Sucesso(true, "Meta removida com sucesso.");
        }

        private async Task<MetaDto> CalcularProgressoAsync(MetaFinanceiraModel meta)
        {
            decimal valorAtual;

            if (meta.Tipo == TipoMeta.Despesa)
            {
                valorAtual = await _repository.GetSomaDespesasAsync(meta.UsuarioId, meta.DataInicio, meta.DataFim, meta.CategoriaDespesaId);
            }
            else
            {
                valorAtual = await _repository.GetSomaReceitasAsync(meta.UsuarioId, meta.DataInicio, meta.DataFim, meta.CategoriaReceitaId);
            }

            decimal percentual = meta.ValorMeta > 0 ? valorAtual / meta.ValorMeta * 100 : 0;
            percentual = Math.Round(percentual, 2);

            string status;
            if (meta.Tipo == TipoMeta.Despesa)
            {
                status = percentual >= 100 ? "excedido" : percentual >= 75 ? "atencao" : "no_limite";
            }
            else
            {
                status = percentual >= 100 ? "atingida" : percentual >= 50 ? "em_andamento" : "abaixo";
            }

            var dto = _mapper.Map<MetaDto>(meta);
            dto.ValorAtual = valorAtual;
            dto.Percentual = percentual;
            dto.StatusMeta = status;

            return dto;
        }
    }
}
