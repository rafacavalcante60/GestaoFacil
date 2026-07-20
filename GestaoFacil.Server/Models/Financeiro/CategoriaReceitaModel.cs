using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Models.Usuario;
using System.Collections.ObjectModel;

namespace GestaoFacil.Server.Models.Domain
{
    public class CategoriaReceitaModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        //null = categoria de sistema (visivel a todos, somente leitura)
        public int? UsuarioId { get; set; }
        public UsuarioModel? Usuario { get; set; }

        public ICollection<ReceitaModel> Receitas { get; set; } = new Collection<ReceitaModel>();
    }
}
