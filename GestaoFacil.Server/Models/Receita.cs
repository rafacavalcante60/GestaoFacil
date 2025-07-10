using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoFacil.Shared.Enums;

namespace GestaoFacil.Server.Models
{
    public class Receita
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; init; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória.")]
        [Column(TypeName = "datetime")]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida.")]
        [Display(Name = "Categoria")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma forma de pagamento válida.")]
        [Display(Name = "Forma de pagamento")]
        public FormaPagamento FormaPagamento { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int UsuarioId { get; set; }

        [Required]
        public required Usuario Usuario { get; set; }
    }
}
