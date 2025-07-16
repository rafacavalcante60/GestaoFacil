using AutoMapper;
using GestaoFacil.Server.Models;
using GestaoFacil.Server.Repositories.Usuario;
using GestaoFacil.Shared.DTOs.Usuario;
using GestaoFacil.Shared.Responses;

namespace GestaoFacil.Server.Services.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<UsuarioDto>> GetByIdAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ResponseHelper.Falha<UsuarioDto>("Usuário não encontrado.");
            }

            var dto = _mapper.Map<UsuarioDto>(usuario);
            return ResponseHelper.Sucesso(dto);
        }

        public async Task<ResponseModel<List<UsuarioDto>>> GetAllAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            var dtos = _mapper.Map<List<UsuarioDto>>(usuarios);
            return ResponseHelper.Sucesso(dtos);
        }

        public async Task<ResponseModel<bool>> UpdatePerfilAsync(int id, UsuarioUpdateDto dto)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ResponseHelper.Falha<bool>("Usuário não encontrado.");
            }

            _mapper.Map(dto, usuario);
            await _repository.UpdateAsync(usuario);

            return ResponseHelper.Sucesso(true);
        }

        public async Task<ResponseModel<bool>> UpdateAdminAsync(int id, UsuarioAdminUpdateDto dto)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ResponseHelper.Falha<bool>("Usuário não encontrado.");
            }

            _mapper.Map(dto, usuario);
            await _repository.UpdateAsync(usuario);

            return ResponseHelper.Sucesso(true);
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ResponseHelper.Falha<bool>("Usuário não encontrado.");
            }

            await _repository.DeleteAsync(usuario);

            return ResponseHelper.Sucesso(true);
        }
    }
}
