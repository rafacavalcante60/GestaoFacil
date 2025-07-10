using GestaoFacil.Shared.Enums;

namespace GestaoFacil.Shared.Dtos
{
    public class DespesaDto {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public Categoria Categoria { get; set; }
        public decimal Valor { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public string? NomeUsuario { get; set; }
    }
}