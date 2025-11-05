using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Route("Productos")]
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductosController(ApplicationDbContext context) => _context = context;

        private readonly IWebHostEnvironment _env;
        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        private bool IsAjax => Request?.Headers != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        private static readonly string[] ImgExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] DocExt = new[] { ".pdf" };
        private const long MaxFileSize = 10 * 1024 * 1024; // El máximo para cargar es de 10MB
        private const string ImgFolder = "/uploads/img";
        private const string FichasFolder = "/uploads/fichas";

        private string EnsureFolder(string relFolder)
        {
            var full = Path.Combine(_env.WebRootPath, relFolder.TrimStart('/'));
            Directory.CreateDirectory(full);
            return full;
        }

        private async Task<string?> SaveLocalAsync(IFormFile file, string relFolder, string[] allowed)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new InvalidOperationException($"Extensión no permitida ({ext}).");
            if (file.Length > MaxFileSize) throw new InvalidOperationException("Archivo demasiado grande (máx 10MB).");

            var root = EnsureFolder(relFolder);
            var name = $"{Guid.NewGuid():N}{ext}";
            var full = Path.Combine(root, name);

            using (var fs = System.IO.File.Create(full))
                await file.CopyToAsync(fs);

            return $"{relFolder}/{name}".Replace("\\", "/");
        }

        private void DeleteLocalIfOwned(string? url, string relFolder)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            if (!url.StartsWith(relFolder, StringComparison.OrdinalIgnoreCase)) return;
            var full = Path.Combine(_env.WebRootPath, url.TrimStart('/'));
            if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var qry = _context.Producto
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdProveedorPrincipalNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(p =>
                    EF.Functions.Like(p.Nombre, $"%{q}%") ||
                    EF.Functions.Like(p.Sku, $"%{q}%") ||
                    EF.Functions.Like(p.CodigoBarras ?? "", $"%{q}%") ||
                    (p.IdCategoriaNavigation != null && EF.Functions.Like(p.IdCategoriaNavigation.Nombre, $"%{q}%")));
            }

            ViewData["q"] = q;
            return View(await qry.OrderBy(p => p.Nombre).ToListAsync());
        }

        [HttpGet("Detalles/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Producto
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdProveedorPrincipalNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpGet("Crear")]
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(
                _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                    .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                "IdCategoria", "Text");

            ViewData["IdProveedorPrincipal"] = new SelectList(
                _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                    .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                "IdProveedor", "Text");

            return View();
        }

        [Authorize(Roles = "Administrador, Usuario")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,Sku,Nombre,IdCategoria,IdProveedorPrincipal,UnidadMedida,CostoStd,PrecioSugerido,CodigoBarras,Activo")] Producto producto, IFormFile? imagenFile, string? imagenUrl, IFormFile? fichaFile
        )
        {
            if (imagenFile != null && !string.IsNullOrWhiteSpace(imagenUrl))
                ModelState.AddModelError("ImagenPath", "Elegí subir un archivo o indicar una URL, no ambos.");

            if (!string.IsNullOrWhiteSpace(imagenUrl))
            {
                if (!Uri.TryCreate(imagenUrl.Trim(), UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    ModelState.AddModelError("ImagenPath", "La URL de imagen debe ser http/https válida.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdCategoria"] = new SelectList(
                    _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                        .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                    "IdCategoria", "Text", producto.IdCategoria);

                ViewData["IdProveedorPrincipal"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                        .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", producto.IdProveedorPrincipal);

                return View(producto);
            }

            try
            {
                // guardar archivos 
                if (imagenFile != null)
                    producto.ImagenPath = await SaveLocalAsync(imagenFile, ImgFolder, ImgExt);
                else if (!string.IsNullOrWhiteSpace(imagenUrl))
                    producto.ImagenPath = imagenUrl.Trim();

                if (fichaFile != null)
                    producto.FichaTecnicaPath = await SaveLocalAsync(fichaFile, FichasFolder, DocExt);

                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Producto creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo crear: {ex.Message}";

                ViewData["IdCategoria"] = new SelectList(
                    _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                        .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                    "IdCategoria", "Text", producto.IdCategoria);

                ViewData["IdProveedorPrincipal"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                        .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", producto.IdProveedorPrincipal);

                return View(producto);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null) return NotFound();

            // Combos legibles con seleccionado
            ViewData["IdCategoria"] = new SelectList(
                _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                    .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                "IdCategoria", "Text", producto.IdCategoria);

            ViewData["IdProveedorPrincipal"] = new SelectList(
                _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                    .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                "IdProveedor", "Text", producto.IdProveedorPrincipal);

            return View(producto);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Sku,Nombre,IdCategoria,IdProveedorPrincipal,UnidadMedida,CostoStd,PrecioSugerido,CodigoBarras,Activo")] Producto producto

            //entradas de archivos
            , IFormFile? imagenFile, string? imagenUrl, IFormFile? fichaFile
            )
        {
            if (id != producto.IdProducto) return NotFound();

            // validación de elección imagen
            if (imagenFile != null && !string.IsNullOrWhiteSpace(imagenUrl))
                ModelState.AddModelError("ImagenPath", "Elegí subir un archivo o indicar una URL, no ambos.");

            if (!string.IsNullOrWhiteSpace(imagenUrl))
            {
                if (!Uri.TryCreate(imagenUrl.Trim(), UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    ModelState.AddModelError("ImagenPath", "La URL de imagen debe ser http/https válida.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdCategoria"] = new SelectList(
                    _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                        .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                    "IdCategoria", "Text", producto.IdCategoria);

                ViewData["IdProveedorPrincipal"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                        .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", producto.IdProveedorPrincipal);

                return View(producto);
            }

            try
            {
                // mantener paths actuales salvo si se va a reemplazar
                var current = await _context.Producto.AsNoTracking()
                                   .FirstOrDefaultAsync(x => x.IdProducto == id);
                if (current == null) return NotFound();

                var imagenActual = current.ImagenPath;
                var fichaActual = current.FichaTecnicaPath;

                // reemplazo de imagen
                if (imagenFile != null)
                {
                    var nuevo = await SaveLocalAsync(imagenFile, ImgFolder, ImgExt);
                    DeleteLocalIfOwned(imagenActual, ImgFolder);
                    producto.ImagenPath = nuevo;
                }
                else if (!string.IsNullOrWhiteSpace(imagenUrl))
                {
                    DeleteLocalIfOwned(imagenActual, ImgFolder);
                    producto.ImagenPath = imagenUrl.Trim();
                }
                else
                {
                    producto.ImagenPath = imagenActual;
                }

                if (fichaFile != null)
                {
                    var nuevo = await SaveLocalAsync(fichaFile, FichasFolder, DocExt);
                    DeleteLocalIfOwned(fichaActual, FichasFolder);
                    producto.FichaTecnicaPath = nuevo;
                }
                else
                {
                    producto.FichaTecnicaPath = fichaActual;
                }
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Producto modificado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.IdProducto)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo actualizar: {ex.Message}";

                ViewData["IdCategoria"] = new SelectList(
                    _context.Categoria.AsNoTracking().OrderBy(x => x.Nombre)
                        .Select(x => new { x.IdCategoria, Text = x.Nombre }).ToList(),
                    "IdCategoria", "Text", producto.IdCategoria);

                ViewData["IdProveedorPrincipal"] = new SelectList(
                    _context.Proveedor.AsNoTracking().OrderBy(x => x.RazonSocial)
                        .Select(x => new { x.IdProveedor, Text = x.RazonSocial + " (" + x.Cuit + ")" }).ToList(),
                    "IdProveedor", "Text", producto.IdProveedorPrincipal);

                return View(producto);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("Eliminar/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Producto
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdProveedorPrincipalNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto != null)
                {
                    // borrar archivos locales si eran nuestros 
                    DeleteLocalIfOwned(producto.ImagenPath, ImgFolder);
                    DeleteLocalIfOwned(producto.FichaTecnicaPath, FichasFolder);
                    _context.Producto.Remove(producto);
                }
                await _context.SaveChangesAsync();

                if (IsAjax) return Json(new { ok = true });
                TempData["toastType"] = "success";
                TempData["toastMsg"] = "Producto eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (IsAjax) return Json(new { ok = false, error = ex.Message });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
                var prod = await _context.Producto
                    .Include(p => p.IdCategoriaNavigation)
                    .Include(p => p.IdProveedorPrincipalNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdProducto == id);
                return View("Delete", prod);
            }
        }

        [HttpPost("ExportarPDF")]
        public async Task<FileResult> ExportarPDF(string? q)
        {
            var data = await _context.Producto
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdProveedorPrincipalNavigation)
                .Where(p => string.IsNullOrWhiteSpace(q) ||
                            EF.Functions.Like(p.Nombre, $"%{q!.Trim()}%") ||
                            EF.Functions.Like(p.Sku, $"%{q!.Trim()}%") ||
                            EF.Functions.Like(p.CodigoBarras ?? "", $"%{q!.Trim()}%") ||
                            (p.IdCategoriaNavigation != null && EF.Functions.Like(p.IdCategoriaNavigation.Nombre, $"%{q!.Trim()}%")))
                .Select(p => new
                {
                    p.IdProducto,
                    p.Sku,
                    p.Nombre,
                    Categoria = p.IdCategoriaNavigation != null ? p.IdCategoriaNavigation.Nombre : "-",
                    Proveedor = p.IdProveedorPrincipalNavigation != null ? p.IdProveedorPrincipalNavigation.RazonSocial : "-",
                    p.UnidadMedida,
                    p.CostoStd,
                    p.PrecioSugerido,
                    p.Activo
                })
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(15);
                    p.DefaultTextStyle(x => x.FontSize(9));
                    p.Header().Text("Productos").SemiBold().FontSize(16).AlignCenter();
                    p.Content().Table(t =>
                    {
                        t.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(35);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                            cols.ConstantColumn(50);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(45);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(H).Text("ID");
                            h.Cell().Element(H).Text("SKU");
                            h.Cell().Element(H).Text("Nombre");
                            h.Cell().Element(H).Text("Categoría");
                            h.Cell().Element(H).Text("Proveedor");
                            h.Cell().Element(H).Text("UM");
                            h.Cell().Element(H).Text("Costo");
                            h.Cell().Element(H).Text("Precio");
                            h.Cell().Element(H).Text("Activo");
                        });

                        foreach (var r in data)
                        {
                            t.Cell().Element(B).Text(r.IdProducto.ToString());
                            t.Cell().Element(B).Text(r.Sku ?? "-");
                            t.Cell().Element(B).Text(r.Nombre ?? "-");
                            t.Cell().Element(B).Text(r.Categoria ?? "-");
                            t.Cell().Element(B).Text(r.Proveedor ?? "-");
                            t.Cell().Element(B).Text(r.UnidadMedida ?? "-");
                            t.Cell().Element(B).Text(r.CostoStd.ToString("N2"));
                            t.Cell().Element(B).Text(r.PrecioSugerido.ToString("N2"));
                            t.Cell().Element(B).Text(r.Activo ? "Sí" : "No");
                        }

                        static IContainer H(IContainer c) => c.Padding(3).Background(Colors.Grey.Lighten3).Border(1);
                        static IContainer B(IContainer c) => c.Padding(3).BorderBottom(1);
                    });
                    p.Footer().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Productos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private bool ProductoExists(int id) => _context.Producto.Any(e => e.IdProducto == id);
    }
}