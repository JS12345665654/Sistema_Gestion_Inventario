using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("Categorias")]
    public class CategoriasController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public CategoriasController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var qry = _context.Categoria.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(c =>
                    EF.Functions.Like(c.Nombre, $"%{q}%") ||
                    EF.Functions.Like(c.Descripcion ?? "", $"%{q}%"));
            }
            ViewData["q"] = q;
            return View(await qry.OrderBy(c => c.Nombre).ToListAsync());
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var categoria = await _context.Categoria.AsNoTracking().FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [Authorize(Roles = "Administrador, Usuario")]
        [HttpGet("Crear")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Administrador,Usuario")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoria,Nombre,Descripcion,Activo")] Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            try
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Categoría creada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";
                return View(categoria);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoria,Nombre,Descripcion,Activo")] Categoria categoria)
        {
            if (id != categoria.IdCategoria) return NotFound();
            if (!ModelState.IsValid) return View(categoria);

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Categoría modificada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(categoria.IdCategoria)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";
                return View(categoria);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var categoria = await _context.Categoria.AsNoTracking().FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var categoria = await _context.Categoria.FindAsync(id);
                if (categoria != null) _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Categoría eliminada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var c = await _context.Categoria.AsNoTracking().FirstOrDefaultAsync(x => x.IdCategoria == id);
                return View("Delete", c);
            }
        }

        [HttpPost("ExportarPDF")]
        public async Task<FileResult> ExportarPDF(string? q)
        {
            var data = await _context.Categoria
                .Where(c => string.IsNullOrWhiteSpace(q) ||
                            EF.Functions.Like(c.Nombre, $"%{q!.Trim()}%") ||
                            EF.Functions.Like(c.Descripcion ?? "", $"%{q!.Trim()}%"))
                .Select(c => new { c.IdCategoria, c.Nombre, c.Descripcion, c.Activo })
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(cn =>
            {
                cn.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(20);
                    p.DefaultTextStyle(x => x.FontSize(10));
                    p.Header().Text("Categorías").SemiBold().FontSize(16).AlignCenter();
                    p.Content().Table(t =>
                    {
                        t.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(5);
                            cols.ConstantColumn(50);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(H).Text("ID");
                            h.Cell().Element(H).Text("Nombre");
                            h.Cell().Element(H).Text("Descripción");
                            h.Cell().Element(H).Text("Activo");
                        });

                        foreach (var r in data)
                        {
                            t.Cell().Element(B).Text(r.IdCategoria.ToString());
                            t.Cell().Element(B).Text(r.Nombre ?? "-");
                            t.Cell().Element(B).Text(r.Descripcion ?? "-");
                            t.Cell().Element(B).Text(r.Activo ? "Sí" : "No");
                        }

                        static IContainer H(IContainer c) => c.Padding(3).Background(Colors.Grey.Lighten3).Border(1);
                        static IContainer B(IContainer c) => c.Padding(3).BorderBottom(1);
                    });
                    p.Footer().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Categorias_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private bool CategoriaExists(int id) => _context.Categoria.Any(e => e.IdCategoria == id);
    }
}