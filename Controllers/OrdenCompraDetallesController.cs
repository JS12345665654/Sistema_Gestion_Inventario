using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("OrdenesCompra/Detalles")]
    public class OrdenCompraDetallesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public OrdenCompraDetallesController(ApplicationDbContext context) => _context = context;

        private bool IsAjax =>
            Request?.Headers != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var q = _context.OrdenCompraDetalle
                .Include(o => o.IdOrdenCompraNavigation)
                .Include(o => o.IdProductoNavigation)
                .AsNoTracking()
                .OrderByDescending(o => o.IdOrdenCompraDetalle);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["Total"] = total;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            return View(items);
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.OrdenCompraDetalle
                .Include(o => o.IdOrdenCompraNavigation)
                .Include(o => o.IdProductoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdOrdenCompraDetalle == id);

            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Crear")]
        public IActionResult Create()
        {
            ViewData["IdOrdenCompra"] = new SelectList(
                _context.OrdenCompra.AsNoTracking()
                    .Include(o => o.IdProveedorNavigation)
                    .OrderBy(o => o.NumeroOc)
                    .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                    .ToList(),
                "IdOrdenCompra", "Text");

            ViewData["IdProducto"] = new SelectList(
                _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                    .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                "IdProducto", "Text");

            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrdenCompraDetalle,IdOrdenCompra,IdProducto,CantidadPedida,CantidadRecibida,CostoUnitario,ImpuestoPct,DescuentoPct")] OrdenCompraDetalle d)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdOrdenCompra"] = new SelectList(
                    _context.OrdenCompra.AsNoTracking()
                        .Include(o => o.IdProveedorNavigation)
                        .OrderBy(o => o.NumeroOc)
                        .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                        .ToList(),
                    "IdOrdenCompra", "Text", d.IdOrdenCompra);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", d.IdProducto);

                return View(d);
            }

            try
            {
                _context.Add(d);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Ítem creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";

                ViewData["IdOrdenCompra"] = new SelectList(
                    _context.OrdenCompra.AsNoTracking()
                        .Include(o => o.IdProveedorNavigation)
                        .OrderBy(o => o.NumeroOc)
                        .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                        .ToList(),
                    "IdOrdenCompra", "Text", d.IdOrdenCompra);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", d.IdProducto);

                return View(d);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var d = await _context.OrdenCompraDetalle.FindAsync(id);
            if (d == null) return NotFound();

            ViewData["IdOrdenCompra"] = new SelectList(
                _context.OrdenCompra.AsNoTracking()
                    .Include(o => o.IdProveedorNavigation)
                    .OrderBy(o => o.NumeroOc)
                    .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                    .ToList(),
                "IdOrdenCompra", "Text", d.IdOrdenCompra);

            ViewData["IdProducto"] = new SelectList(
                _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                    .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                "IdProducto", "Text", d.IdProducto);

            return View(d);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrdenCompraDetalle,IdOrdenCompra,IdProducto,CantidadPedida,CantidadRecibida,CostoUnitario,ImpuestoPct,DescuentoPct")] OrdenCompraDetalle d)
        {
            if (id != d.IdOrdenCompraDetalle) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["IdOrdenCompra"] = new SelectList(
                    _context.OrdenCompra.AsNoTracking()
                        .Include(o => o.IdProveedorNavigation)
                        .OrderBy(o => o.NumeroOc)
                        .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                        .ToList(),
                    "IdOrdenCompra", "Text", d.IdOrdenCompra);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", d.IdProducto);

                return View(d);
            }

            try
            {
                _context.Update(d);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Ítem modificado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.OrdenCompraDetalle.AnyAsync(x => x.IdOrdenCompraDetalle == id);
                if (!exists) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";

                ViewData["IdOrdenCompra"] = new SelectList(
                    _context.OrdenCompra.AsNoTracking()
                        .Include(o => o.IdProveedorNavigation)
                        .OrderBy(o => o.NumeroOc)
                        .Select(o => new { o.IdOrdenCompra, Text = o.NumeroOc + " – " + o.IdProveedorNavigation.RazonSocial })
                        .ToList(),
                    "IdOrdenCompra", "Text", d.IdOrdenCompra);

                ViewData["IdProducto"] = new SelectList(
                    _context.Producto.AsNoTracking().OrderBy(p => p.Sku)
                        .Select(p => new { p.IdProducto, Text = p.Sku + " – " + p.Nombre }).ToList(),
                    "IdProducto", "Text", d.IdProducto);

                return View(d);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var d = await _context.OrdenCompraDetalle
                .Include(o => o.IdOrdenCompraNavigation)
                .Include(o => o.IdProductoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdOrdenCompraDetalle == id);

            if (d == null) return NotFound();
            return View(d);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var d = await _context.OrdenCompraDetalle.FindAsync(id);
                if (d != null) _context.OrdenCompraDetalle.Remove(d);
                await _context.SaveChangesAsync();

                if (IsAjax) return Json(new { ok = true });
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Ítem eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (IsAjax) return Json(new { ok = false, error = ex.Message });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var d = await _context.OrdenCompraDetalle
                    .Include(o => o.IdOrdenCompraNavigation)
                    .Include(o => o.IdProductoNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.IdOrdenCompraDetalle == id);
                return View("Delete", d);
            }
        }

        [HttpPost("ExportarExcel")]
        public async Task<FileResult> ExportarExcel()
        {
            var data = await _context.OrdenCompraDetalle
                .Include(o => o.IdOrdenCompraNavigation)
                .Include(o => o.IdProductoNavigation)
                .AsNoTracking()
                .OrderByDescending(o => o.IdOrdenCompraDetalle)
                .Select(o => new
                {
                    o.IdOrdenCompraDetalle,
                    Orden = o.IdOrdenCompraNavigation != null ? o.IdOrdenCompraNavigation.NumeroOc : "-",
                    Producto = o.IdProductoNavigation != null ? o.IdProductoNavigation.Nombre : "-",
                    o.CantidadPedida,
                    o.CantidadRecibida,
                    o.CostoUnitario,
                    o.ImpuestoPct,
                    o.DescuentoPct
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("OC Detalles");
            ws.Cell(1, 1).InsertTable(data);
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OC_Detalles_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}