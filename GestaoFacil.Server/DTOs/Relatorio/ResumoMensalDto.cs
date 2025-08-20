namespace GestaoFacil.Server.DTOs.Relatorio
{
    public class ResumoMensalDto
    {
        public int Mes { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo { get; set; }
    }
}
