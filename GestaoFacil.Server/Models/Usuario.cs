using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.Comum;

        //relacionamento
        public List<Receita> Receitas { get; set; } = new();
        public List<Despesa> Despesas { get; set; } = new();
    }

    public enum TipoUsuario
    {
        Comum = 0,
        Admin = 1
    }
}
