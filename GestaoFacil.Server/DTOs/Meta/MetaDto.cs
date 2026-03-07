using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.DTOs.Meta
{
    public class MetaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal ValorMeta { get; set; }
        public TipoMeta Tipo { get; set; }
        public int? CategoriaDespesaId { get; set; }
        public int? CategoriaReceitaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Ativo { get; set; }

        // Calculados pelo service
        public decimal ValorAtual { get; set; }
        public decimal Percentual { get; set; }
        public string StatusMeta { get; set; } = string.Empty;
    }
}
