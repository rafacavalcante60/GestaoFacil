using AutoMapper;
using GestaoFacil.Server.Models;
using GestaoFacil.Server.Repositories.Usuario;
using GestaoFacil.Server.DTOs.Usuario;
using GestaoFacil.Server.Responses;
using Microsoft.Extensions.Logging;

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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário {Id}", id);
                return ResponseHelper.Falha<UsuarioDto>("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ResponseModel<List<UsuarioDto>>> GetRecentAsync()
        {
            try
            {
                var usuarios = await _repository.GetRecentAsync();
                var dtos = _mapper.Map<List<UsuarioDto>>(usuarios);
                return ResponseHelper.Sucesso(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários recentes");
                return ResponseHelper.Falha<List<UsuarioDto>>("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ResponseModel<bool>> UpdatePerfilAsync(int id, UsuarioUpdateDto dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuário {Id}", id);
                return ResponseHelper.Falha<bool>("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ResponseModel<bool>> UpdateAdminAsync(int id, UsuarioAdminUpdateDto dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário {Id} por administrador", id);
                return ResponseHelper.Falha<bool>("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ResponseModel<bool>> DeleteAdminAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário {Id}", id);
                return ResponseHelper.Falha<bool>("Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
