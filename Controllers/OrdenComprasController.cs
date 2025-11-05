using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("OrdenesCompra")]
    public class OrdenComprasController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public OrdenComprasController(ApplicationDbContext context) => _context = context;

        private bool IsAjax =>
            Request?.Headers != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.OrdenCompra
                .Include(o => o.IdAlmacenRecepcionNavigation)
                .Include(o => o.IdProveedorNavigation)
                .AsNoTracking()
                .OrderByDescending(o => o.FechaEmision)
                .ThenByDescending(o => o.IdOrdenCompra)
                .ToListAsync();

            return View(items);
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ordenCompra = await _context.OrdenCompra
                .Include(o => o.IdAlmacenRecepcionNavigation)
                .Include(o => o.IdProveedorNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdOrdenCompra == id);

            if (ordenCompra == null) return NotFound();
            return View(ordenCompra);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Crear")]
        public IActionResult Create()
        {
            ViewData["IdAlmacenRecepcion"] = new SelectList(
                _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                    .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                "IdAlmacen", "Text");

            ViewData["IdProveedor"] = new SelectList(
                _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                    .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                "IdProveedor", "Text");

            return View("~/Views/OrdenCompras/Create.cshtml", new OrdenCompra());
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrdenCompra,NumeroOc,IdProveedor,FechaEmision,FechaEsperada,Estado,Moneda,Observaciones,IdAlmacenRecepcion")] OrdenCompra oc)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdAlmacenRecepcion"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", oc.IdAlmacenRecepcion);

                ViewData["IdProveedor"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                        .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", oc.IdProveedor);

                return View("~/Views/OrdenCompras/Create.cshtml", oc);
            }

            try
            {
                _context.Add(oc);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Orden creada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";

                ViewData["IdAlmacenRecepcion"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", oc.IdAlmacenRecepcion);

                ViewData["IdProveedor"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                        .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", oc.IdProveedor);

                return View("~/Views/OrdenCompras/Create.cshtml", oc);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var oc = await _context.OrdenCompra.FindAsync(id);
            if (oc == null) return NotFound();

            ViewData["IdAlmacenRecepcion"] = new SelectList(
                _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                    .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                "IdAlmacen", "Text", oc.IdAlmacenRecepcion);

            ViewData["IdProveedor"] = new SelectList(
                _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                    .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                "IdProveedor", "Text", oc.IdProveedor);

            return View("~/Views/OrdenCompras/Edit.cshtml", oc);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [HttpPost("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrdenCompra,NumeroOc,IdProveedor,FechaEmision,FechaEsperada,Estado,Moneda,Observaciones,IdAlmacenRecepcion")] OrdenCompra oc)
        {
            if (id == 0) id = oc.IdOrdenCompra;
            if (id != oc.IdOrdenCompra) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdAlmacenRecepcion"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", oc.IdAlmacenRecepcion);

                ViewData["IdProveedor"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                        .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", oc.IdProveedor);

                return View("~/Views/OrdenCompras/Edit.cshtml", oc);
            }

            try
            {
                _context.Update(oc);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Orden modificada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.OrdenCompra.AnyAsync(x => x.IdOrdenCompra == id);
                if (!exists) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";

                ViewData["IdAlmacenRecepcion"] = new SelectList(
                    _context.Almacen.AsNoTracking().OrderBy(a => a.Codigo)
                        .Select(a => new { a.IdAlmacen, Text = a.Codigo + " - " + a.Nombre }).ToList(),
                    "IdAlmacen", "Text", oc.IdAlmacenRecepcion);

                ViewData["IdProveedor"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(p => p.RazonSocial)
                        .Select(p => new { p.IdProveedor, Text = p.RazonSocial + " (" + p.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", oc.IdProveedor);

                return View("~/Views/OrdenCompras/Edit.cshtml", oc);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var oc = await _context.OrdenCompra
                .Include(o => o.IdAlmacenRecepcionNavigation)
                .Include(o => o.IdProveedorNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdOrdenCompra == id);

            if (oc == null) return NotFound();
            return View("~/Views/OrdenCompras/Delete.cshtml", oc);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var oc = await _context.OrdenCompra
                    .Include(o => o.Detalles)
                    .FirstOrDefaultAsync(o => o.IdOrdenCompra == id);

                if (oc == null) return NotFound();

                var detIds = oc.Detalles.Select(d => d.IdOrdenCompraDetalle).ToList();
                var tieneMovs = detIds.Any() && await _context.MovimientoInventario
                    .AnyAsync(m => m.IdOrdenCompraDetalle != null && detIds.Contains(m.IdOrdenCompraDetalle.Value));

                if (tieneMovs)
                {
                    TempData["toastType"] = "error";
                    TempData["toastMsg"] = "No se puede eliminar la OC: tiene movimientos de inventario vinculados.";
                    return RedirectToAction("Details", new { id });
                }

                if (oc.Detalles.Any())
                    _context.RemoveRange(oc.Detalles);

                _context.OrdenCompra.Remove(oc);
                await _context.SaveChangesAsync();

                if (IsAjax) return Json(new { ok = true });
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Orden eliminada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (IsAjax) return Json(new { ok = false, error = ex.Message });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var oc = await _context.OrdenCompra
                    .Include(o => o.IdAlmacenRecepcionNavigation)
                    .Include(o => o.IdProveedorNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.IdOrdenCompra == id);
                return View("~/Views/OrdenCompras/Delete.cshtml", oc);
            }
        }

        [HttpPost("ExportarExcel")]
        public async Task<FileResult> ExportarExcel()
        {
            var data = await _context.OrdenCompra
                .Include(o => o.IdAlmacenRecepcionNavigation)
                .Include(o => o.IdProveedorNavigation)
                .AsNoTracking()
                .OrderByDescending(o => o.FechaEmision)
                .Select(o => new
                {
                    o.IdOrdenCompra,
                    o.NumeroOc,
                    Proveedor = o.IdProveedorNavigation != null ? o.IdProveedorNavigation.RazonSocial : "-",
                    Almacen = o.IdAlmacenRecepcionNavigation != null ? o.IdAlmacenRecepcionNavigation.Codigo : "-",
                    o.Estado,
                    o.Moneda,
                    o.FechaEmision,
                    o.FechaEsperada
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Ordenes");
            ws.Cell(1, 1).InsertTable(data);
            ws.Column(7).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Column(8).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrdenesCompra_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}