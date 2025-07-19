namespace GestaoFacil.Shared.DTOs.Auth
{
    public class TokenDto
    {
        public string? Token { get; set; }
        public DateTime ExpiraEm { get; set; }
    }
}