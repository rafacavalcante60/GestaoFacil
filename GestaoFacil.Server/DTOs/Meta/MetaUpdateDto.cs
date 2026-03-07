using GestaoFacil.Server.Models.Principais;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Meta
{
    public class MetaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 9999999.99, ErrorMessage = "O valor da meta deve ser maior que zero.")]
        public decimal ValorMeta { get; set; }

        [Required]
        public TipoMeta Tipo { get; set; }

        public int? CategoriaDespesaId { get; set; }
        public int? CategoriaReceitaId { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        public bool Ativo { get; set; }
    }
}
