using AutoMapper;
using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models;
using GestaoFacil.Server.Repositories;
using GestaoFacil.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Services
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

        public async Task<List<DespesaDto>> GetAllByUsuarioAsync(int usuarioId)
        {
            var despesas = await _repository.GetAllByUsuarioAsync(usuarioId);
            return _mapper.Map<List<DespesaDto>>(despesas);
        }

        public async Task<DespesaDto?> GetByIdAsync(int id, int usuarioId)
        {
            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            return despesa == null ? null : _mapper.Map<DespesaDto>(despesa);
        }

        public async Task<DespesaDto> CreateAsync(DespesaCreateDto dto, int usuarioId)
        {
            var despesa = _mapper.Map<Despesa>(dto);
            despesa.UsuarioId = usuarioId;

            await _repository.AddAsync(despesa);

            var criada = await _repository.GetByIdAsync(despesa.Id, usuarioId);
            return _mapper.Map<DespesaDto>(criada);
        }

        public async Task<bool> UpdateAsync(int id, DespesaUpdateDto dto, int usuarioId)
        {
            if (id != dto.Id) return false;

            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            if (despesa == null) return false;

            _mapper.Map(dto, despesa);
            await _repository.UpdateAsync(despesa);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            var despesa = await _repository.GetByIdAsync(id, usuarioId);
            if (despesa == null) return false;

            await _repository.DeleteAsync(despesa);
            return true;
        }
    }
}
