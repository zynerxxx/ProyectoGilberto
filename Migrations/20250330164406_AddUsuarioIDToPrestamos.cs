using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIDToPrestamos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioID",
                table: "Prestamos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_LibroID",
                table: "Prestamos",
                column: "LibroID");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_UsuarioID",
                table: "Prestamos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_AutorID",
                table: "Libros",
                column: "AutorID");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_CategoriaID",
                table: "Libros",
                column: "CategoriaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Autores_AutorID",
                table: "Libros",
                column: "AutorID",
                principalTable: "Autores",
                principalColumn: "AutorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Categorias_CategoriaID",
                table: "Libros",
                column: "CategoriaID",
                principalTable: "Categorias",
                principalColumn: "CategoriaID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Libros_LibroID",
                table: "Prestamos",
                column: "LibroID",
                principalTable: "Libros",
                principalColumn: "LibroID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioID",
                table: "Prestamos",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Autores_AutorID",
                table: "Libros");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Categorias_CategoriaID",
                table: "Libros");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Libros_LibroID",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioID",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_LibroID",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_UsuarioID",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Libros_AutorID",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_Libros_CategoriaID",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Prestamos");
        }
    }
}
