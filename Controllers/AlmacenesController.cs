using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("Almacenes")]
    public class AlmacenesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public AlmacenesController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var qry = _context.Almacen.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(a =>
                    EF.Functions.Like(a.Codigo, $"%{q}%") ||
                    EF.Functions.Like(a.Nombre, $"%{q}%") ||
                    EF.Functions.Like(a.Direccion ?? "", $"%{q}%"));
            }
            ViewData["q"] = q;
            return View(await qry.OrderBy(a => a.Nombre).ToListAsync());
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var almacen = await _context.Almacen.AsNoTracking().FirstOrDefaultAsync(m => m.IdAlmacen == id);
            if (almacen == null) return NotFound();
            return View(almacen);
        }

        [HttpGet("Crear")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAlmacen,Codigo,Nombre,Direccion,Activo")] Almacen almacen)
        {
            if (!ModelState.IsValid) return View(almacen);

            try
            {
                _context.Add(almacen);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Almacén creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";
                return View(almacen);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var almacen = await _context.Almacen.FindAsync(id);
            if (almacen == null) return NotFound();
            return View(almacen);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAlmacen,Codigo,Nombre,Direccion,Activo")] Almacen almacen)
        {
            if (id != almacen.IdAlmacen) return NotFound();
            if (!ModelState.IsValid) return View(almacen);

            try
            {
                _context.Update(almacen);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Almacén modificado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlmacenExists(almacen.IdAlmacen)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";
                return View(almacen);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var almacen = await _context.Almacen.AsNoTracking().FirstOrDefaultAsync(m => m.IdAlmacen == id);
            if (almacen == null) return NotFound();
            return View(almacen);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var almacen = await _context.Almacen.FindAsync(id);
                if (almacen != null) _context.Almacen.Remove(almacen);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Almacén eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var a = await _context.Almacen.AsNoTracking().FirstOrDefaultAsync(x => x.IdAlmacen == id);
                return View("Delete", a);
            }
        }

        [HttpPost("ExportarPDF")]
        public async Task<FileResult> ExportarPDF(string? q)
        {
            var data = await _context.Almacen
                .Where(a => string.IsNullOrWhiteSpace(q) ||
                            EF.Functions.Like(a.Codigo, $"%{q.Trim()}%") ||
                            EF.Functions.Like(a.Nombre, $"%{q.Trim()}%") ||
                            EF.Functions.Like(a.Direccion ?? "", $"%{q.Trim()}%"))
                .Select(a => new { a.IdAlmacen, a.Codigo, a.Nombre, a.Direccion, a.Activo })
                .AsNoTracking()
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(20);
                    p.DefaultTextStyle(x => x.FontSize(10));
                    p.Header().Text("Almacenes").SemiBold().FontSize(16).AlignCenter();
                    p.Content().Table(t =>
                    {
                        t.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(4);
                            cols.ConstantColumn(50);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(H).Text("ID");
                            h.Cell().Element(H).Text("Código");
                            h.Cell().Element(H).Text("Nombre");
                            h.Cell().Element(H).Text("Dirección");
                            h.Cell().Element(H).Text("Activo");
                        });

                        foreach (var r in data)
                        {
                            t.Cell().Element(B).Text(r.IdAlmacen.ToString());
                            t.Cell().Element(B).Text(r.Codigo ?? "-");
                            t.Cell().Element(B).Text(r.Nombre ?? "-");
                            t.Cell().Element(B).Text(r.Direccion ?? "-");
                            t.Cell().Element(B).Text(r.Activo ? "Sí" : "No");
                        }

                        static IContainer H(IContainer c) => c.Padding(3).Background(Colors.Grey.Lighten3).Border(1);
                        static IContainer B(IContainer c) => c.Padding(3).BorderBottom(1);
                    });
                    p.Footer().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Almacenes_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private bool AlmacenExists(int id) => _context.Almacen.Any(e => e.IdAlmacen == id);
    }
}