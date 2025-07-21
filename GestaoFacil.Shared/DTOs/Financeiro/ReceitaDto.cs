namespace GestaoFacil.Shared.DTOs.Financeiro
{
    public class ReceitaDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public int CategoriaReceitaId { get; set; }
        public int FormaPagamentoId { get; set; }
        public string? NomeUsuario { get; set; }
    }
}
