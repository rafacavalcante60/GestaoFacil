using AutoMapper;
using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models;
using GestaoFacil.Server.Repositories;
using GestaoFacil.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Services
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

        public async Task<List<ReceitaDto>> GetAllByUsuarioAsync(int usuarioId)
        {
            var receitas = await _repository.GetAllByUsuarioAsync(usuarioId);
            return _mapper.Map<List<ReceitaDto>>(receitas);
        }

        public async Task<ReceitaDto?> GetByIdAsync(int id, int usuarioId)
        {
            var receita = await _repository.GetByIdAsync(id, usuarioId);
            return receita == null ? null : _mapper.Map<ReceitaDto>(receita);
        }

        public async Task<ReceitaDto> CreateAsync(ReceitaCreateDto dto, int usuarioId)
        {
            var receita = _mapper.Map<Receita>(dto);
            receita.UsuarioId = usuarioId;

            await _repository.AddAsync(receita);

            var criada = await _repository.GetByIdAsync(receita.Id, usuarioId);
            return _mapper.Map<ReceitaDto>(criada);
        }

        public async Task<bool> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId)
        {
            if (id != dto.Id) return false;

            var receita = await _repository.GetByIdAsync(id, usuarioId);
            if (receita == null) return false;

            _mapper.Map(dto, receita);
            await _repository.UpdateAsync(receita);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            var receita = await _repository.GetByIdAsync(id, usuarioId);
            if (receita == null) return false;

            await _repository.DeleteAsync(receita);
            return true;
        }
    }

}
