namespace GestaoFacil.Server.DTOs.Relatorio
{
    public class ResumoFinanceiroDto
    {
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => TotalReceitas - TotalDespesas;
    }
}
