using GestaoFacil.Server.Models.Usuario;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoFacil.Server.Models.Principais
{
    public class MetaFinanceiraModel
    {
        public int Id { get; init; }
        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorMeta { get; set; }

        public TipoMeta Tipo { get; set; }

        public int? CategoriaDespesaId { get; set; }
        public int? CategoriaReceitaId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DataInicio { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DataFim { get; set; }

        public bool Ativo { get; set; } = true;

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; } = null!;
    }
}
