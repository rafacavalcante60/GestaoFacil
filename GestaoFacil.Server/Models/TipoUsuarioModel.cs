using GestaoFacil.Server.Models.Principais;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.Models
{
    public class TipoUsuarioModel
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public List<UsuarioModel> Usuarios { get; set; } = new();
    }
}
