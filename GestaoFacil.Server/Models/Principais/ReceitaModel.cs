using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoFacil.Server.Models.Principais
{
    public class ReceitaModel
    {
        public int Id { get; init; }

        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime Data { get; set; }

        public string? Descricao { get; set; }

        public int CategoriaReceitaId { get; set; }
        public CategoriaReceitaModel CategoriaReceita{ get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        public int FormaPagamentoId { get; set; }
        public FormaPagamentoModel FormaPagamento { get; set; } = null!;

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; } = null!;
    }
}