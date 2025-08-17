using AutoMapper;
using GestaoFacil.Server.Data;
using GestaoFacil.Server.DTOs.Auth;
using GestaoFacil.Server.Models.Auth;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Server.Responses;
using GestaoFacil.Server.Services.Email;
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
        private readonly IEmailService _emailService;

        public AuthService(IUsuarioRepository usuarioRepository, TokenService tokenService, ILogger<AuthService> logger, 
            IMapper mapper, AppDbContext context, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
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

            var criado = await _usuarioRepository.AddAsync(usuario);

            _logger.LogInformation("Usuário registrado com sucesso. Id: {Id}, Email: {Email}", criado.Id, criado.Email);
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

        public async Task<ResponseModel<string>> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
        {
            _logger.LogInformation("Solicitação de esqueci a senha para: {Email}", dto.Email);

            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (usuario == null)
            {
                _logger.LogWarning("Email não encontrado no esqueci a senha: {Email}", dto.Email);
                return ResponseHelper.Sucesso("Se o email existir, um link de redefinição foi enviado.");
            }

            var token = _tokenService.GeneratePasswordResetToken();
            var tokenExpiraEm = DateTime.UtcNow.AddHours(1);

            usuario.PasswordResetToken = token;
            usuario.PasswordResetTokenExpiraEm = tokenExpiraEm;
            await _usuarioRepository.UpdateAsync(usuario);

            var linkReset = $"https://seusite.com/reset-password?token={token}"; //necessario alteracao pós criação de front-end

            try
            {
                await _emailService.SendAsync(
                    to: usuario.Email,
                    subject: "Redefinição de senha GestaoFacil",
                    body: $"Olá, <br> Clique no link abaixo para redefinir sua senha:<br><a href='{linkReset}'>{linkReset}</a><br>Este link expira em 1 hora."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar email de redefinição para {Email}", dto.Email);
                return ResponseHelper.Falha<string>("Não foi possível enviar o email de redefinição. Tente novamente mais tarde.");
            }

            _logger.LogInformation("Token de redefinição gerado e email enviado para usuário ID {UsuarioId}", usuario.Id);
            return ResponseHelper.Sucesso("Se o email existir, um link de redefinição foi enviado.");
        }

        public async Task<ResponseModel<string>> ResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            _logger.LogInformation("Tentativa de redefinição de senha com token: {Token}", dto.Token);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == dto.Token &&
                u.PasswordResetTokenExpiraEm > DateTime.UtcNow);

            if (usuario == null)
            {
                _logger.LogWarning("Token inválido ou expirado para redefinição de senha.");
                return ResponseHelper.Falha<string>("Token inválido ou expirado.");
            }

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            usuario.PasswordResetToken = null;
            usuario.PasswordResetTokenExpiraEm = null;

            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Senha redefinida com sucesso para o usuário ID {UsuarioId}", usuario.Id);
            return ResponseHelper.Sucesso("Senha redefinida com sucesso.");
        }

    }
}
