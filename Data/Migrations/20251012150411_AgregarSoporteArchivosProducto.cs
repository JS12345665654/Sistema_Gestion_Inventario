using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Inventario.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSoporteArchivosProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FichaTecnicaPath",
                table: "Producto",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPath",
                table: "Producto",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProveedorIdProveedor",
                table: "Producto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "OrdenCompraDetalle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ProveedorIdProveedor",
                table: "Producto",
                column: "ProveedorIdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalle_ProductoIdProducto",
                table: "OrdenCompraDetalle",
                column: "ProductoIdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenCompraDetalle_Producto_ProductoIdProducto",
                table: "OrdenCompraDetalle",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Proveedor_ProveedorIdProveedor",
                table: "Producto",
                column: "ProveedorIdProveedor",
                principalTable: "Proveedor",
                principalColumn: "IdProveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenCompraDetalle_Producto_ProductoIdProducto",
                table: "OrdenCompraDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Proveedor_ProveedorIdProveedor",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_ProveedorIdProveedor",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_OrdenCompraDetalle_ProductoIdProducto",
                table: "OrdenCompraDetalle");

            migrationBuilder.DropColumn(
                name: "FichaTecnicaPath",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "ImagenPath",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "ProveedorIdProveedor",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "OrdenCompraDetalle");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "AspNetUsers");
        }
    }
}
