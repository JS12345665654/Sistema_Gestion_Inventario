using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("Proveedores")]
    public class ProveedoresController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public ProveedoresController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var qry = _context.Proveedor.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(p =>
                    EF.Functions.Like(p.RazonSocial, $"%{q}%") ||
                    EF.Functions.Like(p.Cuit, $"%{q}%") ||
                    EF.Functions.Like(p.Email ?? "", $"%{q}%") ||
                    EF.Functions.Like(p.Ciudad ?? "", $"%{q}%"));
            }
            ViewData["q"] = q;
            return View(await qry.OrderBy(p => p.RazonSocial).ToListAsync());
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedor.AsNoTracking().FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpGet("Crear")]
        public IActionResult Create() => View();

        [HttpPost("Crear")]
        [Authorize(Roles = "Administrador, Usuario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProveedor,RazonSocial,Cuit,Email,Telefono,Direccion,Ciudad,Provincia,Cp,Activo,LeadTimeDias")] Proveedor proveedor)
        {
            if (!ModelState.IsValid) return View(proveedor);

            try
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Proveedor creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";
                return View(proveedor);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdProveedor,RazonSocial,Cuit,Email,Telefono,Direccion,Ciudad,Provincia,Cp,Activo,LeadTimeDias")] Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor) return NotFound();
            if (!ModelState.IsValid) return View(proveedor);

            try
            {
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Proveedor modificado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(proveedor.IdProveedor)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";
                return View(proveedor);
            }
        }

        [HttpGet("Eliminar/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedor.AsNoTracking().FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var proveedor = await _context.Proveedor.FindAsync(id);
                if (proveedor != null) _context.Proveedor.Remove(proveedor);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Proveedor eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var p = await _context.Proveedor.AsNoTracking().FirstOrDefaultAsync(x => x.IdProveedor == id);
                return View("Delete", p);
            }
        }

        [HttpPost("ExportarPDF")]
        public async Task<FileResult> ExportarPDF(string? q)
        {
            var data = await _context.Proveedor
                .Where(p => string.IsNullOrWhiteSpace(q) ||
                            EF.Functions.Like(p.RazonSocial, $"%{q!.Trim()}%") ||
                            EF.Functions.Like(p.Cuit, $"%{q!.Trim()}%") ||
                            EF.Functions.Like(p.Email ?? "", $"%{q!.Trim()}%") ||
                            EF.Functions.Like(p.Ciudad ?? "", $"%{q!.Trim()}%"))
                .Select(p => new
                {
                    p.IdProveedor,
                    p.RazonSocial,
                    CUIT = p.Cuit,
                    p.Email,
                    p.Telefono,
                    p.Ciudad,
                    p.Provincia,
                    p.Activo
                })
                .AsNoTracking()
                .OrderBy(p => p.RazonSocial)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(20);
                    p.DefaultTextStyle(x => x.FontSize(10));
                    p.Header().Text("Proveedores").SemiBold().FontSize(16).AlignCenter();
                    p.Content().Table(t =>
                    {
                        t.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                            cols.ConstantColumn(50);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(H).Text("ID");
                            h.Cell().Element(H).Text("Razón Social");
                            h.Cell().Element(H).Text("CUIT");
                            h.Cell().Element(H).Text("Email");
                            h.Cell().Element(H).Text("Teléfono");
                            h.Cell().Element(H).Text("Ciudad");
                            h.Cell().Element(H).Text("Provincia");
                            h.Cell().Element(H).Text("Activo");
                        });

                        foreach (var r in data)
                        {
                            t.Cell().Element(B).Text(r.IdProveedor.ToString());
                            t.Cell().Element(B).Text(r.RazonSocial ?? "-");
                            t.Cell().Element(B).Text(r.CUIT ?? "-");
                            t.Cell().Element(B).Text(r.Email ?? "-");
                            t.Cell().Element(B).Text(r.Telefono ?? "-");
                            t.Cell().Element(B).Text(r.Ciudad ?? "-");
                            t.Cell().Element(B).Text(r.Provincia ?? "-");
                            t.Cell().Element(B).Text(r.Activo ? "Sí" : "No");
                        }

                        static IContainer H(IContainer c) => c.Padding(3).Background(Colors.Grey.Lighten3).Border(1);
                        static IContainer B(IContainer c) => c.Padding(3).BorderBottom(1);
                    });
                    p.Footer().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Proveedores_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private bool ProveedorExists(int id) => _context.Proveedor.Any(e => e.IdProveedor == id);
    }
}