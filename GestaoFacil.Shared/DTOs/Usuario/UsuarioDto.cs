namespace GestaoFacil.Shared.DTOs.Usuario
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TipoUsuarioId { get; set; }
        public string TipoUsuarioNome { get; set; } = string.Empty;
    }
}
