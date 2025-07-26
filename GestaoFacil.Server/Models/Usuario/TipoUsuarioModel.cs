using System.Collections.ObjectModel;

namespace GestaoFacil.Server.Models.Usuario
{
    public class TipoUsuarioModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public ICollection<UsuarioModel> Usuarios { get; set; } = new Collection<UsuarioModel>();
    }
}
