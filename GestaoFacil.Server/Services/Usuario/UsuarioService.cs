using AutoMapper;
using GestaoFacil.Server.DTOs.Despesa;
using GestaoFacil.Server.DTOs.Usuario;
using GestaoFacil.Server.Pagination;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository repository, IMapper mapper, ILogger<UsuarioService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseModel<UsuarioDto>> GetByIdAsync(int id)
        {
                var usuario = await _repository.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuário {Id} não encontrado", id);
                    return ResponseHelper.Falha<UsuarioDto>("Usuário não encontrado.");
                }

                var dto = _mapper.Map<UsuarioDto>(usuario);
                return ResponseHelper.Sucesso(dto);
            }

        public async Task<ResponseModel<PagedList<UsuarioDto>>> GetPagedAsync(Parameters parameters)
        {
            var usuarios = await _repository.GetPagedAsync(parameters.PageNumber, parameters.PageSize);

            var dtos = new PagedList<UsuarioDto>(
                _mapper.Map<List<UsuarioDto>>(usuarios),
                usuarios.TotalCount,
                usuarios.CurrentPage,
                usuarios.PageSize
            );

            return ResponseHelper.Sucesso(dtos, "Usuários paginados carregados com sucesso.");
        }


        public async Task<ResponseModel<bool>> UpdatePerfilAsync(int id, UsuarioUpdateDto dto)
        {
                var usuario = await _repository.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de atualizar perfil de usuário {Id} que não existe", id);
                    return ResponseHelper.Falha<bool>("Usuário não encontrado.");
                }

                _mapper.Map(dto, usuario);
                await _repository.UpdateAsync(usuario);

                _logger.LogInformation("Perfil do usuário {Id} atualizado com sucesso", id);
                return ResponseHelper.Sucesso(true);
            }

        public async Task<ResponseModel<bool>> UpdateAdminAsync(int id, UsuarioAdminUpdateDto dto)
        {
                var usuario = await _repository.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de admin atualizar usuário {Id} que não existe", id);
                    return ResponseHelper.Falha<bool>("Usuário não encontrado.");
                }

                _mapper.Map(dto, usuario);
                await _repository.UpdateAsync(usuario);

                _logger.LogInformation("Usuário {Id} atualizado por administrador", id);
                return ResponseHelper.Sucesso(true);
            }

        public async Task<ResponseModel<bool>> DeleteAdminAsync(int id)
        {
                var usuario = await _repository.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de deletar usuário {Id} que não existe", id);
                    return ResponseHelper.Falha<bool>("Usuário não encontrado.");
                }

                await _repository.DeleteAsync(usuario);

                _logger.LogInformation("Usuário {Id} deletado com sucesso", id);
                return ResponseHelper.Sucesso(true);
            }
    }
}
