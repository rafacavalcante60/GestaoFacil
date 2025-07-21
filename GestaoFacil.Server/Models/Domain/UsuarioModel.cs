using GestaoFacil.Server.Models.Domain;
using System.Collections.ObjectModel;

namespace GestaoFacil.Server.Models.Principais
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public int TipoUsuarioId { get; set; }
        public TipoUsuarioModel TipoUsuario { get; set; } = null!;

        public ICollection<ReceitaModel> Receitas { get; set; } = new Collection<ReceitaModel>();
        public ICollection<DespesaModel> Despesas { get; set; } = new Collection<DespesaModel>();
    }
}
