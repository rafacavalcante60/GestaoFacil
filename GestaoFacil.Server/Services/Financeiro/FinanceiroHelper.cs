using ClosedXML.Excel;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Financeiro
{
    public static class FinanceiroHelper
    {
        public static ResponseModel<T>? ValidarFiltroData<T>(DateTime? inicio, DateTime? fim, ILogger logger, int usuarioId)
        {
            if (inicio.HasValue && fim.HasValue && inicio > fim)
            {
                logger.LogWarning("Filtro inválido: DataInicial {DataInicial} maior que DataFinal {DataFinal} para usuário {UsuarioId}",
                    inicio, fim, usuarioId);
                return ResponseHelper.Falha<T>("A data inicial não pode ser maior que a data final.");
            }
            return null;
        }

        public static byte[] GerarExcel<T>(
            List<T> items,
            string sheetName,
            Func<T, string> getData,
            Func<T, string> getNome,
            Func<T, string> getDescricao,
            Func<T, string> getCategoria,
            Func<T, string> getFormaPagamento,
            Func<T, decimal> getValor)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            var headers = new[] { "Data", "Nome", "Descrição", "Categoria", "Forma Pagamento", "Valor" };
            for (int c = 0; c < headers.Length; c++)
            {
                worksheet.Cell(1, c + 1).Value = headers[c];
                worksheet.Cell(1, c + 1).Style.Font.Bold = true;
                worksheet.Cell(1, c + 1).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                worksheet.Cell(1, c + 1).Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                worksheet.Cell(i + 2, 1).Value = getData(item);
                worksheet.Cell(i + 2, 2).Value = getNome(item);
                worksheet.Cell(i + 2, 3).Value = getDescricao(item);
                worksheet.Cell(i + 2, 4).Value = getCategoria(item);
                worksheet.Cell(i + 2, 5).Value = getFormaPagamento(item);
                worksheet.Cell(i + 2, 6).Value = getValor(item);
                worksheet.Cell(i + 2, 6).Style.NumberFormat.Format = "R$ #,##0.00";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
