using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public List<ReceitaModel> Receitas { get; set; } = new();
        public List<DespesaModel> Despesas { get; set; } = new();
    }
}
