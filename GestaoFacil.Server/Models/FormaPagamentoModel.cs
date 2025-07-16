using GestaoFacil.Server.Models.Principais;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.Models
{
    public class FormaPagamentoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da forma de pagamento é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public List<DespesaModel> Despesas { get; set; } = new();
        public List<ReceitaModel> Receitas { get; set; } = new();
    }
}
