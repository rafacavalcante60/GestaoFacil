using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Models.Auth
{
    public class RefreshTokenModel
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public bool EstaRevogado { get; set; }

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; } = null!;
    }
}
