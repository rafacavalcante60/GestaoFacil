using GestaoFacil.Server.Models.Principais;
using System.Collections.ObjectModel;

namespace GestaoFacil.Server.Models.Domain
{
    public class TipoUsuarioModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public ICollection<UsuarioModel> Usuarios { get; set; } = new Collection<UsuarioModel>();
    }
}
