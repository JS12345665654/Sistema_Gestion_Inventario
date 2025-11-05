using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Route("Inventario/Salida")]
    public class SalidasController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SalidasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class SalidaVM
        {
            public int IdProducto { get; set; }
            public int IdAlmacen { get; set; }
            public decimal Cantidad { get; set; }
            public short Motivo { get; set; } = 2; // 2=Venta, 3=Consumo
            public string? Referencia { get; set; }
            public string? Observaciones { get; set; }
        }

        [HttpGet("Crear")]
        public async Task<IActionResult> Crear()
        {
            await CargarCombos();
            return View(new SalidaVM());
        }

        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SalidaVM vm)
        {
            await CargarCombos();

            if (vm.Cantidad <= 0)
                ModelState.AddModelError(nameof(vm.Cantidad), "La cantidad debe ser mayor a cero.");

            var prod = await _context.Producto.AsNoTracking().FirstOrDefaultAsync(p => p.IdProducto == vm.IdProducto);
            if (prod == null) ModelState.AddModelError(nameof(vm.IdProducto), "Producto inválido.");

            var alm = await _context.Almacen.AsNoTracking().FirstOrDefaultAsync(a => a.IdAlmacen == vm.IdAlmacen);
            if (alm == null) ModelState.AddModelError(nameof(vm.IdAlmacen), "Almacén inválido.");

            if (vm.Motivo != 2 && vm.Motivo != 3)
                ModelState.AddModelError(nameof(vm.Motivo), "Motivo inválido.");

            if (!ModelState.IsValid) return View(vm);

            var inQty = await _context.MovimientoInventario
                .Where(m => m.IdProducto == vm.IdProducto && m.IdAlmacen == vm.IdAlmacen && m.Tipo == "IN")
                .SumAsync(m => (decimal?)m.Cantidad) ?? 0m;

            var outQty = await _context.MovimientoInventario
                .Where(m => m.IdProducto == vm.IdProducto && m.IdAlmacen == vm.IdAlmacen && m.Tipo == "OUT")
                .SumAsync(m => (decimal?)m.Cantidad) ?? 0m;

            var disponible = inQty - outQty;
            if (vm.Cantidad > disponible)
            {
                ModelState.AddModelError(nameof(vm.Cantidad), $"Stock insuficiente. Disponible: {disponible:N2}.");
                return View(vm);
            }

            _context.MovimientoInventario.Add(new MovimientoInventario
            {
                IdProducto = vm.IdProducto,
                IdAlmacen = vm.IdAlmacen,
                Tipo = "OUT",
                Motivo = vm.Motivo,           // 2=Venta, 3=Consumo
                Cantidad = vm.Cantidad,
                CostoUnitario = null,         // opcional: usar prod.CostoStd
                IdOrdenCompraDetalle = null,  // sólo se exige en Motivo=1 => Compra
                FechaMovimiento = DateTime.UtcNow,
                Referencia = vm.Referencia,
                IdUsuarioOperador = User?.Identity?.Name, 
                Observaciones = vm.Observaciones
            });

            await _context.SaveChangesAsync();

            TempData["toastType"] = "success";
            TempData["toastMsg"] = "Salida registrada. Stock actualizado.";
            return RedirectToAction("Index", "Stock");
        }

        private async Task CargarCombos()
        {
            ViewBag.Productos = await _context.Producto
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProducto.ToString(),
                    Text = $"{p.Sku} - {p.Nombre}"
                }).ToListAsync();

            ViewBag.Almacenes = await _context.Almacen
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem
                {
                    Value = a.IdAlmacen.ToString(),
                    Text = $"{a.Codigo} - {a.Nombre}"
                }).ToListAsync();

            ViewBag.Motivos = new List<SelectListItem>
            {
                new SelectListItem{ Value = "2", Text = "Venta (OUT)" },
                new SelectListItem{ Value = "3", Text = "Consumo interno (OUT)" }
            };
        }
    }
}