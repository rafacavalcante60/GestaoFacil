using GestaoFacil.Server.Models.Usuario;

public interface ITokenService
{
    (string token, DateTime expiraEm) GenerateToken(UsuarioModel usuario);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken();
}