using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Filtro
{
    public class ReceitaFiltroDto
    {
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal? ValorMin { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal? ValorMax { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        [Range(1, 5, ErrorMessage = "O ID da categoria deve ser maior que zero.")]
        public int? CategoriaReceitaId { get; set; }

        [Range(1, 7, ErrorMessage = "O ID da forma de pagamento deve ser maior que zero.")]
        public int? FormaPagamentoId { get; set; }

        [MaxLength(300, ErrorMessage = "O texto de busca pode ter no máximo 300 caracteres.")]
        public string? BuscaTexto { get; set; }
    }
}
