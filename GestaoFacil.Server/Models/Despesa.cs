using GestaoFacil.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoFacil.Server.Models
{
    public class Despesa
    {
        public int Id { get; init; }

        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime Data { get; set; }

        public string? Descricao { get; set; }

        public Categoria Categoria { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public int UsuarioId { get; set; }

        public required Usuario Usuario { get; set; }
    }
}
