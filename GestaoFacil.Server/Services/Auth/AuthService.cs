using AutoMapper;
using GestaoFacil.Server.Migrations;
using GestaoFacil.Server.Models;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Repositories.Usuario;
using GestaoFacil.Shared.DTOs.Auth;
using GestaoFacil.Shared.Responses;
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

        public AuthService(IUsuarioRepository usuarioRepository, TokenService tokenService, ILogger<AuthService> logger, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseModel<TokenDto>> LoginAsync(UsuarioLoginDto request)
        {
            _logger.LogInformation("Tentando login: {Email}", request.Email);

            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                return ResponseHelper.Falha<TokenDto>("Email inválido.");
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                return ResponseHelper.Falha<TokenDto>("Email ou senha inválidos.");
            }

            var (token, expiraEm) = _tokenService.GenerateToken(usuario);

            var tokenDto = new TokenDto
            {
                Token = token,
                ExpiraEm = expiraEm
            };

            return ResponseHelper.Sucesso(tokenDto, "Login realizado com sucesso.");
        }

        public async Task<ResponseModel<string>> RegisterAsync(UsuarioRegisterDto dto)
        {
            if (await _usuarioRepository.EmailExistsAsync(dto.Email))
            {
                return ResponseHelper.Falha<string>("Email já está em uso.");
            }

            var tipoComum = await _usuarioRepository.GetTipoUsuarioByNameAsync("Comum");
            if (tipoComum is null)
            {
                return ResponseHelper.Falha<string>("Tipo de usuário padrão não configurado.");
            }

            var usuario = _mapper.Map<UsuarioModel>(dto);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            usuario.TipoUsuarioId = tipoComum.Id;

            await _usuarioRepository.AddAsync(usuario);

            return ResponseHelper.Sucesso("Usuário registrado com sucesso.");
        }

    }
}
