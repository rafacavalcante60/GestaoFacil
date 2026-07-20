using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoFacil.Server.Migrations
{
    /// <inheritdoc />
    public partial class CategoriasPorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CategoriasReceita",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "CategoriasReceita",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CategoriasDespesa",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "CategoriasDespesa",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasReceita_UsuarioId_Nome",
                table: "CategoriasReceita",
                columns: new[] { "UsuarioId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasDespesa_UsuarioId_Nome",
                table: "CategoriasDespesa",
                columns: new[] { "UsuarioId", "Nome" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriasDespesa_Usuarios_UsuarioId",
                table: "CategoriasDespesa",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriasReceita_Usuarios_UsuarioId",
                table: "CategoriasReceita",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriasDespesa_Usuarios_UsuarioId",
                table: "CategoriasDespesa");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriasReceita_Usuarios_UsuarioId",
                table: "CategoriasReceita");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasReceita_UsuarioId_Nome",
                table: "CategoriasReceita");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasDespesa_UsuarioId_Nome",
                table: "CategoriasDespesa");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CategoriasReceita");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CategoriasDespesa");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CategoriasReceita",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CategoriasDespesa",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
