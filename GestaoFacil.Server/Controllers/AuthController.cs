using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Server.Services;
using GestaoFacil.Server.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using GestaoFacil.Server.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using GestaoFacil.Shared.DTOs.Auth; // Namespace dos DTOs

namespace GestaoFacil.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(TokenService tokenService, AppDbContext context, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de login para o email: {Email}", request.Email);

                if (!new EmailAddressAttribute().IsValid(request.Email))
                {
                    _logger.LogWarning("Email inválido fornecido: {Email}", request.Email);
                    return BadRequest("Email inválido.");
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
                {
                    _logger.LogWarning("Tentativa de login falhou para o email: {Email}", request.Email);
                    return Unauthorized("Email ou senha inválidos.");
                }

                // Agora GenerateToken retorna uma tupla (token, expiraEm)
                var (token, expiraEm) = _tokenService.GenerateToken(usuario);

                _logger.LogInformation("Login bem-sucedido para o email: {Email}", request.Email);

                return Ok(new LoginResponse
                {
                    Token = token,
                    ExpiraEm = expiraEm
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login para o email: {Email}", request.Email);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de registro para o email: {Email}", request.Email);

                if (string.IsNullOrEmpty(request.Nome))
                {
                    _logger.LogWarning("Nome não fornecido durante o registro.");
                    return BadRequest("Nome é obrigatório.");
                }
                if (!new EmailAddressAttribute().IsValid(request.Email))
                {
                    _logger.LogWarning("Email inválido fornecido: {Email}", request.Email);
                    return BadRequest("Email inválido.");
                }
                if (string.IsNullOrEmpty(request.Senha) || request.Senha.Length < 6)
                {
                    _logger.LogWarning("Senha inválida fornecida.");
                    return BadRequest("Senha deve ter pelo menos 6 caracteres.");
                }

                if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                {
                    _logger.LogWarning("Email já está em uso: {Email}", request.Email);
                    return BadRequest("Email já está em uso.");
                }

                string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

                var usuario = new Usuario
                {
                    Nome = request.Nome,
                    Email = request.Email,
                    SenhaHash = senhaHash,
                    TipoUsuario = TipoUsuario.Comum
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário registrado com sucesso: {Email}", request.Email);
                return Ok("Usuário registrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário com o email: {Email}", request.Email);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }
    }
}
