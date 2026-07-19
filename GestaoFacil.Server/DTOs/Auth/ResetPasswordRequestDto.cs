using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string Token { get; set; } = null!;

        // Mesma regra do cadastro (UsuarioRegisterDto): sem isto o reset era um
        // caminho para contornar a politica de senha e gravar uma senha de 1 caractere.
        [Required, MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NewPassword { get; set; } = null!;
    }
}
