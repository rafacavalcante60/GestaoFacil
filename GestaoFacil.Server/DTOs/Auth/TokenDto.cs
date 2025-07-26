namespace GestaoFacil.Server.DTOs.Auth
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}