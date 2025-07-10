//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using GestaoFacil.Server.Models;
//using Microsoft.IdentityModel.Tokens;

//namespace GestaoFacil.Server.Services
//{
//    public class TokenService
//    {
//        private readonly IConfiguration _configuration;

//        public TokenService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public string GenerateToken(Usuario usuario)
//        {
//            // Cria um manipulador de tokens JWT
//            var tokenHandler = new JwtSecurityTokenHandler();

//            // Obtém a chave secreta do appsettings.json e converte para bytes
//            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

//            // Define as "claims" (informações) que serão incluídas no token
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, usuario.Email), // Adiciona o email do usuário
//                new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()) // Adiciona o tipo de usuário (role)
//            };

//            // Cria o descritor do token
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(claims), // Define as claims
//                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])), // Define o tempo de expiração
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key), // Define a chave de assinatura
//                    SecurityAlgorithms.HmacSha256Signature) // Define o algoritmo de assinatura
//            };

//            // Gera o token
//            var token = tokenHandler.CreateToken(tokenDescriptor);

//            // Retorna o token como uma string
//            return tokenHandler.WriteToken(token);
//        }
//    }
//}

using GestaoFacil.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiraEm) GenerateToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
        };

        var tempoExpiracao = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = tempoExpiracao,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = _configuration["Jwt:Audience"],
            Issuer = _configuration["Jwt:Issuer"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), tempoExpiracao);
    }
}
