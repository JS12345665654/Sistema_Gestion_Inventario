using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Inventario.Data.Migrations
{
    public partial class AgregarColorHexaEtiqueta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Etiqueta','ColorHex') IS NULL
    ALTER TABLE Etiqueta ADD ColorHex NVARCHAR(7) NULL;
");

            migrationBuilder.Sql(@"
UPDATE Etiqueta SET ColorHex = '#6c757d' WHERE Nombre = N'Sin Ficha' AND (ColorHex IS NULL OR ColorHex = '');
UPDATE Etiqueta SET ColorHex = '#0d6efd' WHERE Nombre = N'Imagen Remota' AND (ColorHex IS NULL OR ColorHex = '');
UPDATE Etiqueta SET ColorHex = '#dc3545' WHERE Nombre = N'Bajo Stock' AND (ColorHex IS NULL OR ColorHex = '');
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Etiqueta','ColorHex') IS NOT NULL
    ALTER TABLE Etiqueta DROP COLUMN ColorHex;
");
        }
    }
}