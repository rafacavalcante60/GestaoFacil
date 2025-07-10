using GestaoFacil.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Shared.Dtos
{
    public class DespesaCreateDto {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; }

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida.")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma forma de pagamento válida.")]
        public FormaPagamento FormaPagamento { get; set; }
    }
}