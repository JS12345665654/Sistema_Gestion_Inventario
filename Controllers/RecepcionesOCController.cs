using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Route("OC/Recepcion")]
    public class RecepcionesOCController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecepcionesOCController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===== VM que solo se usa en esta vista =====
        public class RecepcionOCVM
        {
            public int IdOrdenCompra { get; set; }
            public string NumeroOc { get; set; } = string.Empty;

            [Display(Name = "Almacén de recepción")]
            public int? IdAlmacenRecepcion { get; set; }

            public List<LineaVM> Lineas { get; set; } = new();
        }

        public class LineaVM
        {
            public int IdOrdenCompraDetalle { get; set; }
            public string Producto { get; set; } = string.Empty;
            public string Sku { get; set; } = string.Empty;

            [Display(Name = "Pedida")] public decimal CantidadPedida { get; set; }
            [Display(Name = "Recibida")] public decimal CantidadRecibida { get; set; }
            [Display(Name = "Pendiente")] public decimal Pendiente => CantidadPedida - CantidadRecibida;

            [Display(Name = "A recibir")]
            [Range(0, double.MaxValue, ErrorMessage = "Cantidad inválida")]
            public decimal ARecibir { get; set; }
            public decimal CostoUnitario { get; set; }
        }

        [HttpGet("Recibir/{id:int}")]
        public async Task<IActionResult> Recibir(int id)
        {
            var oc = await _context.OrdenCompra
                .Include(o => o.IdAlmacenRecepcionNavigation)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(o => o.IdOrdenCompra == id);

            if (oc == null) return NotFound();

            var vm = new RecepcionOCVM
            {
                IdOrdenCompra = oc.IdOrdenCompra,
                NumeroOc = oc.NumeroOc,
                IdAlmacenRecepcion = oc.IdAlmacenRecepcion
            };

            vm.Lineas = oc.Detalles
                .OrderBy(d => d.IdOrdenCompraDetalle)
                .Select(d => new LineaVM
                {
                    IdOrdenCompraDetalle = d.IdOrdenCompraDetalle,
                    Producto = d.IdProductoNavigation.Nombre,
                    Sku = d.IdProductoNavigation.Sku,
                    CantidadPedida = d.CantidadPedida,
                    CantidadRecibida = d.CantidadRecibida,
                    ARecibir = Math.Max(0, d.CantidadPedida - d.CantidadRecibida),
                    CostoUnitario = d.CostoUnitario
                }).ToList();

            ViewBag.Almacenes = await _context.Almacen
                .OrderBy(a => a.Nombre)
                .Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = a.IdAlmacen.ToString(),
                    Text = $"{a.Codigo} - {a.Nombre}"
                }).ToListAsync();

            return View(vm);
        }

        [HttpPost("Recibir/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recibir(int id, RecepcionOCVM vm)
        {
            if (id != vm.IdOrdenCompra) return NotFound();

            var oc = await _context.OrdenCompra
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.IdOrdenCompra == id);
            if (oc == null) return NotFound();

            if (vm.IdAlmacenRecepcion == null)
                ModelState.AddModelError(nameof(vm.IdAlmacenRecepcion), "Seleccioná un almacén de recepción.");

            if (!ModelState.IsValid)
            {
                ViewBag.Almacenes = await _context.Almacen
                    .OrderBy(a => a.Nombre)
                    .Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = a.IdAlmacen.ToString(),
                        Text = $"{a.Codigo} - {a.Nombre}"
                    }).ToListAsync();
                return View(vm);
            }

            oc.IdAlmacenRecepcion = vm.IdAlmacenRecepcion;

            var userId = _userManager.GetUserId(User);
            var ahoraUtc = DateTime.UtcNow;

            foreach (var linea in vm.Lineas)
            {
                if (linea.ARecibir <= 0) continue;

                var det = oc.Detalles.FirstOrDefault(d => d.IdOrdenCompraDetalle == linea.IdOrdenCompraDetalle);
                if (det == null) continue;

                var pendiente = det.CantidadPedida - det.CantidadRecibida;
                var qty = Math.Min(linea.ARecibir, pendiente);
                if (qty <= 0) continue;

                det.CantidadRecibida += qty;

                _context.MovimientoInventario.Add(new MovimientoInventario
                {
                    IdProducto = det.IdProducto,
                    IdAlmacen = vm.IdAlmacenRecepcion!.Value,
                    Tipo = "IN",
                    Motivo = 1,
                    Cantidad = qty,
                    CostoUnitario = det.CostoUnitario,
                    IdOrdenCompraDetalle = det.IdOrdenCompraDetalle,
                    FechaMovimiento = ahoraUtc,
                    IdUsuarioOperador = userId,
                    Referencia = oc.NumeroOc,
                    Observaciones = "Recepción de OC"
                });
            }

            bool completa = oc.Detalles.All(d => d.CantidadRecibida >= d.CantidadPedida);
            oc.Estado = completa ? (short)3 : (short)2;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "OrdenCompras", new { id = oc.IdOrdenCompra });
        }
    }
}
