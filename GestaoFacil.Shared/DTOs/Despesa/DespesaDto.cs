namespace GestaoFacil.Shared.DTOs.Despesa
{
    public class DespesaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public int CategoriaDespesaId { get; set; }
        public string CategoriaDespesaNome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int FormaPagamentoId { get; set; }
        public string FormaPagamentoNome { get; set; } = string.Empty;
        public string? NomeUsuario { get; set; }
    }
}