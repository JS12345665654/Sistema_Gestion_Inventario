using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;         // para ejecutar el SP
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;
using Sistema_Gestion_Inventario.Models;
using System.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("Stock")]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connString;

        public StockController(ApplicationDbContext context, IConfiguration cfg)
        {
            _context = context;
            _connString = cfg.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? idProducto, int? idAlmacen)
        {
            ViewData["IdProducto"] = new SelectList(
                await _context.Producto.AsNoTracking()
                    .OrderBy(p => p.Nombre)
                    .Select(p => new { p.IdProducto, Text = p.Nombre + " (" + p.Sku + ")" })
                    .ToListAsync(),
                "IdProducto", "Text", idProducto);

            ViewData["IdAlmacen"] = new SelectList(
                await _context.Almacen.AsNoTracking()
                    .OrderBy(a => a.Codigo)
                    .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre })
                    .ToListAsync(),
                "IdAlmacen", "Text", idAlmacen);

            var data = await GetStockAsync(idProducto, idAlmacen); // Procedimiento almacenado
            return View(data);
        }

        [HttpPost("ExportarExcel")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<FileResult> ExportarExcel(int? idProducto, int? idAlmacen)
        {
            var data = await GetStockAsync(idProducto, idAlmacen);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Stock");
            ws.Cell(1, 1).InsertTable(data.Select(d => new
            {
                d.IdProducto,
                d.Sku,
                d.Producto,
                d.IdAlmacen,
                d.AlmacenCodigo,
                d.Almacen,
                d.StockActual,
                d.UltimoMovimiento
            }));
            ws.Column(8).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"StockActual_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        //Ejecuta el Stored Procedure Stored_Procedure_de_StockActual 
        private async Task<List<StockActualRow>> GetStockAsync(int? idProducto, int? idAlmacen)
        {
            var list = new List<StockActualRow>();

            await using var cn = new SqlConnection(_connString);
            await cn.OpenAsync();

            await using var cmd = new SqlCommand("SP_StockActual", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@IdProducto", (object?)idProducto ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@IdAlmacen", (object?)idAlmacen ?? DBNull.Value));

            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new StockActualRow
                {
                    IdProducto = rd.GetInt32(rd.GetOrdinal("IdProducto")),
                    Sku = rd["Sku"] as string ?? "",
                    Producto = rd["Producto"] as string ?? "",
                    IdAlmacen = rd.GetInt32(rd.GetOrdinal("IdAlmacen")),
                    AlmacenCodigo = rd["AlmacenCodigo"] as string ?? "",
                    Almacen = rd["Almacen"] as string ?? "",
                    StockActual = rd.GetDecimal(rd.GetOrdinal("StockActual")),
                    UltimoMovimiento = rd.IsDBNull(rd.GetOrdinal("UltimoMovimiento"))
                        ? (DateTime?)null
                        : rd.GetDateTime(rd.GetOrdinal("UltimoMovimiento"))
                });
            }
            return list;
        }
    }
}