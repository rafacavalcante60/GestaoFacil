using GestaoFacil.Server.Models.Principais;
using System.Collections.ObjectModel;

namespace GestaoFacil.Server.Models.Domain
{
    public class FormaPagamentoModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public ICollection<DespesaModel> Despesas { get; set; } = new Collection<DespesaModel>();
        public ICollection<ReceitaModel> Receitas { get; set; } = new Collection<ReceitaModel>();
    }
}
