namespace GestaoFacil.Server.DTOs.Financeiro
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
