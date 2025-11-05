using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Inventario.Data.Migrations
{
    public partial class Stored_Procedure_de_Stock_Anual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Borro si existía (idempotente)
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[SP_StockActual]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_StockActual];
");

            // Crea el procedimiento almacenado
            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[SP_StockActual]
    @IdProducto INT = NULL,
    @IdAlmacen  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.IdProducto,
        p.Sku,
        p.Nombre            AS Producto,
        m.IdAlmacen,
        a.Codigo            AS AlmacenCodigo,
        a.Nombre            AS Almacen,
        CAST(SUM(CASE WHEN m.Tipo = 'IN' THEN m.Cantidad ELSE -m.Cantidad END) AS DECIMAL(18,2)) AS StockActual,
        MAX(m.FechaMovimiento) AS UltimoMovimiento
    FROM MovimientoInventario m
    INNER JOIN Producto p ON p.IdProducto = m.IdProducto
    INNER JOIN Almacen  a ON a.IdAlmacen  = m.IdAlmacen
    WHERE (@IdProducto IS NULL OR m.IdProducto = @IdProducto)
      AND (@IdAlmacen  IS NULL OR m.IdAlmacen  = @IdAlmacen)
    GROUP BY m.IdProducto, p.Sku, p.Nombre, m.IdAlmacen, a.Codigo, a.Nombre
    ORDER BY p.Nombre, a.Codigo;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[SP_StockActual]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_StockActual];
");
        }
    }
}