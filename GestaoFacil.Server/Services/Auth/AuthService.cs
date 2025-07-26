using AutoMapper;
using GestaoFacil.Server.Data;
using GestaoFacil.Server.Models.Auth;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Server.DTOs.Auth;
using GestaoFacil.Server.Responses;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context; //apenas para o refresh token

        public AuthService(IUsuarioRepository usuarioRepository, TokenService tokenService, ILogger<AuthService> logger, IMapper mapper, AppDbContext context)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResponseModel<TokenDto>> LoginAsync(UsuarioLoginDto request)
        {
            _logger.LogInformation("Tentativa de login para o email: {Email}", request.Email);

            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                _logger.LogWarning("Email inválido informado no login: {Email}", request.Email);
                return ResponseHelper.Falha<TokenDto>("Email inválido.");
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                _logger.LogWarning("Credenciais inválidas para login: {Email}", request.Email);
                return ResponseHelper.Falha<TokenDto>("Email ou senha inválidos.");
            }

            var (accessToken, expiraEm) = _tokenService.GenerateToken(usuario);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenModel = new RefreshTokenModel
            {
                Token = refreshToken,
                UsuarioId = usuario.Id,
                ExpiraEm = DateTime.UtcNow.AddDays(7),
                EstaRevogado = false
            };

            _context.RefreshTokens.Add(refreshTokenModel);
            await _context.SaveChangesAsync();

            var tokenDto = new TokenDto
            {
                Token = accessToken,
                ExpiraEm = expiraEm,
                RefreshToken = refreshToken
            };

            _logger.LogInformation("Login realizado com sucesso para o usuário ID {UsuarioId}", usuario.Id);
            return ResponseHelper.Sucesso(tokenDto, "Login realizado com sucesso.");
        }

        public async Task<ResponseModel<string>> RegisterAsync(UsuarioRegisterDto dto)
        {
            if (await _usuarioRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Tentativa de registro com email já em uso: {Email}", dto.Email);
                return ResponseHelper.Falha<string>("Email já está em uso.");
            }

            var tipoComum = await _usuarioRepository.GetTipoUsuarioByNameAsync("Comum");
            if (tipoComum is null)
            {
                _logger.LogError("Tipo de usuário 'Comum' não encontrado durante o registro.");
                return ResponseHelper.Falha<string>("Tipo de usuário padrão não configurado.");
            }

            var usuario = _mapper.Map<UsuarioModel>(dto);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            usuario.TipoUsuarioId = tipoComum.Id;

            await _usuarioRepository.AddAsync(usuario);

            _logger.LogInformation("Usuário registrado com sucesso: {Email}", dto.Email);
            return ResponseHelper.Sucesso("Usuário registrado com sucesso.");
        }

        public async Task<ResponseModel<string>> LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
            {
                _logger.LogWarning("Tentativa de logout com refresh token inexistente: {RefreshToken}", refreshToken);
                return ResponseHelper.Falha<string>("Token não encontrado.");
            }

            token.EstaRevogado = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Logout realizado com sucesso para o refresh token: {RefreshToken}", refreshToken);
            return ResponseHelper.Sucesso("Logout realizado com sucesso.");
        }

        public async Task<ResponseModel<TokenDto>> RefreshTokenAsync(string refreshToken)
        {
            var tokenModel = await _context.RefreshTokens
                .Include(t => t.Usuario)
                .ThenInclude(u => u.TipoUsuario)
                .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.EstaRevogado);

            if (tokenModel == null || tokenModel.ExpiraEm < DateTime.UtcNow)
            {
                _logger.LogWarning("Tentativa de refresh token inválida ou expirada: {RefreshToken}", refreshToken);
                return ResponseHelper.Falha<TokenDto>("Refresh token inválido ou expirado.");
            }

            tokenModel.EstaRevogado = true;

            var (accessToken, expiraEm) = _tokenService.GenerateToken(tokenModel.Usuario);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshTokenModel
            {
                Token = newRefreshToken,
                UsuarioId = tokenModel.UsuarioId,
                ExpiraEm = DateTime.UtcNow.AddDays(7),
                EstaRevogado = false
            });

            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token renovado com sucesso para o usuário ID {UsuarioId}", tokenModel.UsuarioId);

            var dto = new TokenDto
            {
                Token = accessToken,
                ExpiraEm = expiraEm,
                RefreshToken = newRefreshToken
            };

            return ResponseHelper.Sucesso(dto);
        }
    }
}
