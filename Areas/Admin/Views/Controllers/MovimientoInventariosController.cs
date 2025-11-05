using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("MovimientoInventarios")]
    public class MovimientoInventariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MovimientoInventariosController(ApplicationDbContext context) => _context = context;

        private bool IsAjax =>
            Request?.Headers != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        [HttpGet("")]
        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta, int page = 1, int pageSize = 10)
        {
            var q = _context.MovimientoInventario
                .Include(m => m.IdAlmacenNavigation)
                .Include(m => m.IdOrdenCompraDetalleNavigation)
                .Include(m => m.IdProductoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue)
                q = q.Where(m => m.FechaMovimiento >= desde.Value.Date);

            if (hasta.HasValue)
            {
                var to = hasta.Value.Date.AddDays(1);
                q = q.Where(m => m.FechaMovimiento < to);
            }

            q = q.OrderByDescending(m => m.FechaMovimiento);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["Total"] = total;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["desde"] = desde?.ToString("yyyy-MM-dd");
            ViewData["hasta"] = hasta?.ToString("yyyy-MM-dd");

            return View(items);
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.MovimientoInventario
                .Include(m => m.IdAlmacenNavigation)
                .Include(m => m.IdOrdenCompraDetalleNavigation)
                .Include(m => m.IdProductoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);

            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Crear")]
        public IActionResult Create()
        {
            ViewData["IdAlmacen"] = new SelectList(
                _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                    .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                "IdAlmacen", "Text");

            ViewData["IdProducto"] = new SelectList(
                _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                    .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                "IdProducto", "Text");

            ViewData["IdOrdenCompraDetalle"] = new SelectList(
                _context.OrdenCompraDetalle.AsNoTracking()
                    .Include(d => d.IdOrdenCompraNavigation)
                    .Include(d => d.IdProductoNavigation)
                    .OrderBy(d => d.IdOrdenCompra)
                    .Select(d => new
                    {
                        d.IdOrdenCompraDetalle,
                        Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                    }).ToList(),
                "IdOrdenCompraDetalle", "Text");

            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMovimiento,IdProducto,IdAlmacen,Tipo,Motivo,Cantidad,CostoUnitario,IdOrdenCompraDetalle,FechaMovimiento,Referencia,IdUsuarioOperador,Observaciones")] MovimientoInventario m)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdAlmacen"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", m.IdAlmacen);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", m.IdProducto);

                ViewData["IdOrdenCompraDetalle"] = new SelectList(
                    _context.OrdenCompraDetalle.AsNoTracking()
                        .Include(d => d.IdOrdenCompraNavigation)
                        .Include(d => d.IdProductoNavigation)
                        .OrderBy(d => d.IdOrdenCompra)
                        .Select(d => new
                        {
                            d.IdOrdenCompraDetalle,
                            Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                        }).ToList(),
                    "IdOrdenCompraDetalle", "Text", m.IdOrdenCompraDetalle);

                return View(m);
            }

            try
            {
                _context.Add(m);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Movimiento creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";

                ViewData["IdAlmacen"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", m.IdAlmacen);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", m.IdProducto);

                ViewData["IdOrdenCompraDetalle"] = new SelectList(
                    _context.OrdenCompraDetalle.AsNoTracking()
                        .Include(d => d.IdOrdenCompraNavigation)
                        .Include(d => d.IdProductoNavigation)
                        .OrderBy(d => d.IdOrdenCompra)
                        .Select(d => new
                        {
                            d.IdOrdenCompraDetalle,
                            Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                        }).ToList(),
                    "IdOrdenCompraDetalle", "Text", m.IdOrdenCompraDetalle);

                return View(m);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var m = await _context.MovimientoInventario.FindAsync(id);
            if (m == null) return NotFound();

            ViewData["IdAlmacen"] = new SelectList(
                _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                    .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                "IdAlmacen", "Text", m.IdAlmacen);

            ViewData["IdProducto"] = new SelectList(
                _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                    .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                "IdProducto", "Text", m.IdProducto);

            ViewData["IdOrdenCompraDetalle"] = new SelectList(
                _context.OrdenCompraDetalle.AsNoTracking()
                    .Include(d => d.IdOrdenCompraNavigation)
                    .Include(d => d.IdProductoNavigation)
                    .OrderBy(d => d.IdOrdenCompra)
                    .Select(d => new
                    {
                        d.IdOrdenCompraDetalle,
                        Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                    }).ToList(),
                "IdOrdenCompraDetalle", "Text", m.IdOrdenCompraDetalle);

            return View(m);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMovimiento,IdProducto,IdAlmacen,Tipo,Motivo,Cantidad,CostoUnitario,IdOrdenCompraDetalle,FechaMovimiento,Referencia,IdUsuarioOperador,Observaciones")] MovimientoInventario m)
        {
            if (id != m.IdMovimiento) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["IdAlmacen"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", m.IdAlmacen);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", m.IdProducto);

                ViewData["IdOrdenCompraDetalle"] = new SelectList(
                    _context.OrdenCompraDetalle.AsNoTracking()
                        .Include(d => d.IdOrdenCompraNavigation)
                        .Include(d => d.IdProductoNavigation)
                        .OrderBy(d => d.IdOrdenCompra)
                        .Select(d => new
                        {
                            d.IdOrdenCompraDetalle,
                            Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                        }).ToList(),
                    "IdOrdenCompraDetalle", "Text", m.IdOrdenCompraDetalle);

                return View(m);
            }

            try
            {
                _context.Update(m);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Movimiento modificado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.MovimientoInventario.AnyAsync(x => x.IdMovimiento == id);
                if (!exists) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";

                ViewData["IdAlmacen"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", m.IdAlmacen);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", m.IdProducto);

                ViewData["IdOrdenCompraDetalle"] = new SelectList(
                    _context.OrdenCompraDetalle.AsNoTracking()
                        .Include(d => d.IdOrdenCompraNavigation)
                        .Include(d => d.IdProductoNavigation)
                        .OrderBy(d => d.IdOrdenCompra)
                        .Select(d => new
                        {
                            d.IdOrdenCompraDetalle,
                            Text = d.IdOrdenCompraNavigation.NumeroOc + " – " + d.IdProductoNavigation.Sku + " " + d.IdProductoNavigation.Nombre
                        }).ToList(),
                    "IdOrdenCompraDetalle", "Text", m.IdOrdenCompraDetalle);

                return View(m);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var m = await _context.MovimientoInventario
                .Include(x => x.IdAlmacenNavigation)
                .Include(x => x.IdOrdenCompraDetalleNavigation)
                .Include(x => x.IdProductoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdMovimiento == id);

            if (m == null) return NotFound();
            return View(m);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var m = await _context.MovimientoInventario.FindAsync(id);
                if (m != null) _context.MovimientoInventario.Remove(m);
                await _context.SaveChangesAsync();

                if (IsAjax) return Json(new { ok = true });
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Movimiento eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (IsAjax) return Json(new { ok = false, error = ex.Message });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var m = await _context.MovimientoInventario
                    .Include(x => x.IdAlmacenNavigation)
                    .Include(x => x.IdOrdenCompraDetalleNavigation)
                    .Include(x => x.IdProductoNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdMovimiento == id);
                return View("Delete", m);
            }
        }

        [HttpPost("ExportarExcel")]
        public async Task<FileResult> ExportarExcel(DateTime? desde, DateTime? hasta)
        {
            var q = _context.MovimientoInventario
                .Include(m => m.IdAlmacenNavigation)
                .Include(m => m.IdOrdenCompraDetalleNavigation)
                .Include(m => m.IdProductoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (desde.HasValue) q = q.Where(m => m.FechaMovimiento >= desde.Value.Date);
            if (hasta.HasValue) q = q.Where(m => m.FechaMovimiento < hasta.Value.Date.AddDays(1));

            var data = await q
                .OrderByDescending(m => m.FechaMovimiento)
                .Select(m => new
                {
                    m.IdMovimiento,
                    Producto = m.IdProductoNavigation != null ? m.IdProductoNavigation.Nombre : "-",
                    Almacen = m.IdAlmacenNavigation != null ? m.IdAlmacenNavigation.Codigo : "-",
                    m.Tipo,
                    m.Motivo,
                    m.Cantidad,
                    m.CostoUnitario,
                    m.FechaMovimiento,
                    m.Referencia
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Movimientos");
            ws.Cell(1, 1).InsertTable(data);
            ws.Column(8).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Movimientos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}