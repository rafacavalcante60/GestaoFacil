using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Auth
{
    public class UsuarioRegisterDto
    {
        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Insira um email valido")]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required, Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}