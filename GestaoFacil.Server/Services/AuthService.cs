using GestaoFacil.Server.Models;
using GestaoFacil.Server.Repositories;
using GestaoFacil.Shared.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsuarioRepository usuarioRepository, TokenService tokenService, ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Tentando login: {Email}", request.Email);

            if (!new EmailAddressAttribute().IsValid(request.Email))
                return ServiceResult<LoginResponse>.Fail("Email inválido.");

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
                return ServiceResult<LoginResponse>.Fail("Email ou senha inválidos.", 401);

            var (token, expiraEm) = _tokenService.GenerateToken(usuario);
            return ServiceResult<LoginResponse>.Ok(new LoginResponse { Token = token, ExpiraEm = expiraEm });
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Tentando registrar: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ServiceResult<string>.Fail("Nome é obrigatório.");

            if (!new EmailAddressAttribute().IsValid(request.Email))
                return ServiceResult<string>.Fail("Email inválido.");

            if (string.IsNullOrEmpty(request.Senha) || request.Senha.Length < 6)
                return ServiceResult<string>.Fail("Senha deve ter pelo menos 6 caracteres.");

            if (await _usuarioRepository.EmailExistsAsync(request.Email))
                return ServiceResult<string>.Fail("Email já está em uso.");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var novoUsuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                SenhaHash = senhaHash,
                TipoUsuario = TipoUsuario.Comum
            };

            await _usuarioRepository.AddAsync(novoUsuario);
            return ServiceResult<string>.Ok("Usuário registrado com sucesso.");
        }
    }
}
