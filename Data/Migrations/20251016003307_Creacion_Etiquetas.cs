using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Inventario.Data.Migrations
{
    /// <inheritdoc />
    public partial class Creacion_Etiquetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiqueta", x => x.IdEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "ProductoEtiqueta",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoEtiqueta", x => new { x.IdProducto, x.IdEtiqueta });
                    table.ForeignKey(
                        name: "FK_ProdEti_Etiqueta",
                        column: x => x.IdEtiqueta,
                        principalTable: "Etiqueta",
                        principalColumn: "IdEtiqueta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdEti_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Etiqueta_Nombre",
                table: "Etiqueta",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductoEtiqueta_IdEtiqueta",
                table: "ProductoEtiqueta",
                column: "IdEtiqueta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoEtiqueta");

            migrationBuilder.DropTable(
                name: "Etiqueta");
        }
    }
}
