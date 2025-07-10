using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoFacil.Server.Migrations
{
    /// <inheritdoc />
    public partial class MudancaNomeFormaPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormaPagamentoReceita",
                table: "Receitas",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "FormaPagamentoDespesa",
                table: "Despesas",
                newName: "FormaPagamento");

            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento",
                table: "Receitas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Receitas");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Receitas",
                newName: "FormaPagamentoReceita");

            migrationBuilder.RenameColumn(
                name: "FormaPagamento",
                table: "Despesas",
                newName: "FormaPagamentoDespesa");
        }
    }
}
