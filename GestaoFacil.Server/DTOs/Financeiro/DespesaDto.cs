namespace GestaoFacil.Server.DTOs.Despesa
{
    public class DespesaDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public int CategoriaDespesaId { get; set; }
        public int FormaPagamentoId { get; set; }
        public string? NomeUsuario { get; set; }
    }
}