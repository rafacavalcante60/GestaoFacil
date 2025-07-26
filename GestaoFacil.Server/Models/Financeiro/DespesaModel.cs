using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Models.Usuario;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoFacil.Server.Models.Principais
{
    public class DespesaModel
    {
        public int Id { get; init; }

        public string? Nome { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Data { get; set; }

        public string? Descricao { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        public int CategoriaDespesaId { get; set; }
        public CategoriaDespesaModel CategoriaDespesa { get; set; } = null!;

        public int FormaPagamentoId { get; set; }
        public FormaPagamentoModel FormaPagamento { get; set; } = null!;

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; } = null!;
    }

}