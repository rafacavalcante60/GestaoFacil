using GestaoFacil.Server.Models.Principais;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenService
{
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly double _jwtExpireMinutes;

    public TokenService(IConfiguration configuration)
    {
        _jwtKey = configuration["Jwt:Key"]
            ?? throw new ArgumentNullException("Jwt:Key", "A chave JWT não foi definida no appsettings.");

        _jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new ArgumentNullException("Jwt:Issuer", "O issuer JWT não foi definido no appsettings.");

        _jwtAudience = configuration["Jwt:Audience"]
            ?? throw new ArgumentNullException("Jwt:Audience", "A audiência JWT não foi definida no appsettings.");

        var expireValue = configuration["Jwt:ExpireMinutes"];
        if (!double.TryParse(expireValue, out _jwtExpireMinutes))
        {
            throw new ArgumentException("Jwt:ExpireMinutes está ausente ou inválido no appsettings.");
        }
    }

    public (string token, DateTime expiraEm) GenerateToken(UsuarioModel usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
             new Claim(ClaimTypes.Role, usuario.TipoUsuario.Nome)
        };

        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtExpireMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiraEm,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = _jwtAudience,
            Issuer = _jwtIssuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiraEm);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

}
