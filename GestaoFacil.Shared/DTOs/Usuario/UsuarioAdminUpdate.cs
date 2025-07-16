using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Shared.DTOs.Usuario
{
    public class UsuarioAdminUpdateDto
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tipo de usuário inválido.")]
        public int TipoUsuarioId { get; set; }
    }
}
