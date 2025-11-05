using Microsoft.EntityFrameworkCore;

namespace Sistema_Gestion_Inventario.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var ctx = services.GetRequiredService<ApplicationDbContext>();

            // ===== Categorías
            if (!await ctx.Categoria.AnyAsync())
            {
                ctx.Categoria.AddRange(
                    new Categoria { Nombre = "Notebooks", Descripcion = "Portátiles / laptops" },
                    new Categoria { Nombre = "Periféricos", Descripcion = "Teclados, mouse, etc." },
                    new Categoria { Nombre = "Audio", Descripcion = "Auriculares, parlantes" },
                    new Categoria { Nombre = "Monitores", Descripcion = "LCD / LED" },
                    new Categoria { Nombre = "Almacenamiento", Descripcion = "SSD / HDD" }
                );
                await ctx.SaveChangesAsync();
            }

            // ===== Proveedores
            if (!await ctx.Proveedor.AnyAsync())
            {
                ctx.Proveedor.AddRange(
                    new Proveedor { RazonSocial = "Tecno S.A.", Cuit = "30-70000001-9", Email = "ventas@tecno.com", Telefono = "11-4000-1000", Ciudad = "CABA", Provincia = "Buenos Aires", LeadTimeDias = 4 },
                    new Proveedor { RazonSocial = "CompuWorld SRL", Cuit = "30-70000002-7", Email = "comercial@compuworld.com", Telefono = "11-4555-2211", Ciudad = "CABA", Provincia = "Buenos Aires", LeadTimeDias = 6 },
                    new Proveedor { RazonSocial = "AudioMax", Cuit = "30-70000003-5", Email = "info@audiomax.com", Ciudad = "Rosario", Provincia = "Santa Fe", LeadTimeDias = 5 },
                    new Proveedor { RazonSocial = "PeriCenter", Cuit = "30-70000004-3", Email = "ventas@pericenter.com", Ciudad = "Córdoba", Provincia = "Córdoba", LeadTimeDias = 3 },
                    new Proveedor { RazonSocial = "StoragePro", Cuit = "30-70000005-1", Email = "contacto@storagepro.com", Ciudad = "Mendoza", Provincia = "Mendoza", LeadTimeDias = 7 }
                );
                await ctx.SaveChangesAsync();
            }

            // ===== Almacenes
            if (!await ctx.Almacen.AnyAsync())
            {
                ctx.Almacen.AddRange(
                    new Almacen { Codigo = "CABA", Nombre = "Depósito Central", Direccion = "Av. Siempreviva 123" },
                    new Almacen { Codigo = "CBA", Nombre = "Depósito Córdoba", Direccion = "Bv. San Juan 456" },
                    new Almacen { Codigo = "MZA", Nombre = "Depósito Mendoza", Direccion = "San Martín 789" }
                );
                await ctx.SaveChangesAsync();
            }

            // Diccionarios rápidos para FKs
            var cat = await ctx.Categoria.AsNoTracking().ToDictionaryAsync(x => x.Nombre, x => x.IdCategoria);
            var prov = await ctx.Proveedor.AsNoTracking().ToDictionaryAsync(x => x.RazonSocial, x => x.IdProveedor);

            // ===== Productos
            if (!await ctx.Producto.AnyAsync())
            {
                ctx.Producto.AddRange(
                    new Producto { Sku = "NBK-0001", Nombre = "Notebook 14\" i5 8GB", IdCategoria = cat["Notebooks"], IdProveedorPrincipal = prov["Tecno S.A."], UnidadMedida = "un", CostoStd = 350000, PrecioSugerido = 420000, CodigoBarras = "779000000001" },
                    new Producto { Sku = "NBK-0002", Nombre = "Notebook 15\" i7 16GB", IdCategoria = cat["Notebooks"], IdProveedorPrincipal = prov["CompuWorld SRL"], UnidadMedida = "un", CostoStd = 580000, PrecioSugerido = 670000, CodigoBarras = "779000000002" },
                    new Producto { Sku = "MON-0001", Nombre = "Monitor 24\" FHD", IdCategoria = cat["Monitores"], IdProveedorPrincipal = prov["Tecno S.A."], UnidadMedida = "un", CostoStd = 150000, PrecioSugerido = 185000, CodigoBarras = "779000000010" },
                    new Producto { Sku = "MON-0002", Nombre = "Monitor 27\" QHD", IdCategoria = cat["Monitores"], IdProveedorPrincipal = prov["CompuWorld SRL"], UnidadMedida = "un", CostoStd = 240000, PrecioSugerido = 299000, CodigoBarras = "779000000011" },
                    new Producto { Sku = "MOU-0001", Nombre = "Mouse inalámbrico", IdCategoria = cat["Periféricos"], IdProveedorPrincipal = prov["PeriCenter"], UnidadMedida = "un", CostoStd = 5000, PrecioSugerido = 8990, CodigoBarras = "779000000020" },
                    new Producto { Sku = "KB-0001", Nombre = "Teclado mecánico", IdCategoria = cat["Periféricos"], IdProveedorPrincipal = prov["PeriCenter"], UnidadMedida = "un", CostoStd = 22000, PrecioSugerido = 29900, CodigoBarras = "779000000021" },
                    new Producto { Sku = "AUD-0001", Nombre = "Auriculares on-ear", IdCategoria = cat["Audio"], IdProveedorPrincipal = prov["AudioMax"], UnidadMedida = "un", CostoStd = 18000, PrecioSugerido = 24900, CodigoBarras = "779000000030" },
                    new Producto { Sku = "AUD-0002", Nombre = "Parlante BT portátil", IdCategoria = cat["Audio"], IdProveedorPrincipal = prov["AudioMax"], UnidadMedida = "un", CostoStd = 26000, PrecioSugerido = 33900, CodigoBarras = "779000000031" },
                    new Producto { Sku = "SSD-0001", Nombre = "SSD 500GB NVMe", IdCategoria = cat["Almacenamiento"], IdProveedorPrincipal = prov["StoragePro"], UnidadMedida = "un", CostoStd = 48000, PrecioSugerido = 61900, CodigoBarras = "779000000040" },
                    new Producto { Sku = "SSD-0002", Nombre = "SSD 1TB NVMe", IdCategoria = cat["Almacenamiento"], IdProveedorPrincipal = prov["StoragePro"], UnidadMedida = "un", CostoStd = 82000, PrecioSugerido = 104900, CodigoBarras = "779000000041" },
                    new Producto { Sku = "HDD-0001", Nombre = "HDD 2TB 3.5\"", IdCategoria = cat["Almacenamiento"], IdProveedorPrincipal = prov["StoragePro"], UnidadMedida = "un", CostoStd = 70000, PrecioSugerido = 89900, CodigoBarras = "779000000042" },
                    new Producto { Sku = "MOU-0002", Nombre = "Mouse gamer", IdCategoria = cat["Periféricos"], IdProveedorPrincipal = prov["PeriCenter"], UnidadMedida = "un", CostoStd = 9500, PrecioSugerido = 14900, CodigoBarras = "779000000022" }
                );
                await ctx.SaveChangesAsync();
            }

            // ===== Órdenes de compra + detalles + movimientos
            if (!await ctx.OrdenCompra.AnyAsync())
            {
                var alm = await ctx.Almacen.AsNoTracking().ToDictionaryAsync(x => x.Codigo, x => x.IdAlmacen);
                var prod = await ctx.Producto.AsNoTracking().ToDictionaryAsync(x => x.Sku, x => x.IdProducto);

                var oc1 = new OrdenCompra
                {
                    NumeroOc = "OC-2025-0001",
                    IdProveedor = prov["Tecno S.A."],
                    FechaEmision = DateTime.UtcNow.AddDays(-12),
                    FechaEsperada = DateTime.UtcNow.AddDays(-8),
                    Estado = 1,
                    Moneda = "ARS",
                    IdAlmacenRecepcion = alm["CABA"],
                    Observaciones = "Stock inicial notebooks/monitores"
                };
                var oc2 = new OrdenCompra
                {
                    NumeroOc = "OC-2025-0002",
                    IdProveedor = prov["PeriCenter"],
                    FechaEmision = DateTime.UtcNow.AddDays(-9),
                    FechaEsperada = DateTime.UtcNow.AddDays(-6),
                    Estado = 1,
                    Moneda = "ARS",
                    IdAlmacenRecepcion = alm["CBA"],
                    Observaciones = "Periféricos"
                };
                var oc3 = new OrdenCompra
                {
                    NumeroOc = "OC-2025-0003",
                    IdProveedor = prov["StoragePro"],
                    FechaEmision = DateTime.UtcNow.AddDays(-7),
                    FechaEsperada = DateTime.UtcNow.AddDays(-3),
                    Estado = 2,
                    Moneda = "ARS",
                    IdAlmacenRecepcion = alm["MZA"],
                    Observaciones = "Almacenamiento"
                };

                ctx.OrdenCompra.AddRange(oc1, oc2, oc3);
                await ctx.SaveChangesAsync();

                var d = new List<OrdenCompraDetalle>
                {
                    new() { IdOrdenCompra = oc1.IdOrdenCompra, IdProducto = prod["NBK-0001"], CantidadPedida=5,  CantidadRecibida=5,  CostoUnitario=340000, ImpuestoPct=21 },
                    new() { IdOrdenCompra = oc1.IdOrdenCompra, IdProducto = prod["MON-0001"], CantidadPedida=6,  CantidadRecibida=6,  CostoUnitario=145000, ImpuestoPct=21 },

                    new() { IdOrdenCompra = oc2.IdOrdenCompra, IdProducto = prod["MOU-0001"], CantidadPedida=20, CantidadRecibida=20, CostoUnitario=4800,  ImpuestoPct=21 },
                    new() { IdOrdenCompra = oc2.IdOrdenCompra, IdProducto = prod["KB-0001"],  CantidadPedida=12, CantidadRecibida=12, CostoUnitario=21000, ImpuestoPct=21 },

                    new() { IdOrdenCompra = oc3.IdOrdenCompra, IdProducto = prod["SSD-0001"], CantidadPedida=15, CantidadRecibida=10, CostoUnitario=47000, ImpuestoPct=21 },
                    new() { IdOrdenCompra = oc3.IdOrdenCompra, IdProducto = prod["HDD-0001"], CantidadPedida=10, CantidadRecibida=5,  CostoUnitario=69000, ImpuestoPct=21 }
                };
                ctx.OrdenCompraDetalle.AddRange(d);
                await ctx.SaveChangesAsync();

                var movs = new List<MovimientoInventario>
                {
                    new() { IdProducto= d[0].IdProducto, IdAlmacen= alm["CABA"], Tipo="IN", Motivo=1, Cantidad=d[0].CantidadRecibida, CostoUnitario=d[0].CostoUnitario, IdOrdenCompraDetalle=d[0].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-8), Referencia=$"Recepción OC {oc1.NumeroOc}" },
                    new() { IdProducto= d[1].IdProducto, IdAlmacen= alm["CABA"], Tipo="IN", Motivo=1, Cantidad=d[1].CantidadRecibida, CostoUnitario=d[1].CostoUnitario, IdOrdenCompraDetalle=d[1].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-8), Referencia=$"Recepción OC {oc1.NumeroOc}" },

                    new() { IdProducto= d[2].IdProducto, IdAlmacen= alm["CBA"],  Tipo="IN", Motivo=1, Cantidad=d[2].CantidadRecibida, CostoUnitario=d[2].CostoUnitario, IdOrdenCompraDetalle=d[2].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-6), Referencia=$"Recepción OC {oc2.NumeroOc}" },
                    new() { IdProducto= d[3].IdProducto, IdAlmacen= alm["CBA"],  Tipo="IN", Motivo=1, Cantidad=d[3].CantidadRecibida, CostoUnitario=d[3].CostoUnitario, IdOrdenCompraDetalle=d[3].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-6), Referencia=$"Recepción OC {oc2.NumeroOc}" },

                    new() { IdProducto= d[4].IdProducto, IdAlmacen= alm["MZA"],  Tipo="IN", Motivo=1, Cantidad=d[4].CantidadRecibida, CostoUnitario=d[4].CostoUnitario, IdOrdenCompraDetalle=d[4].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-3), Referencia=$"Recepción OC {oc3.NumeroOc}" },
                    new() { IdProducto= d[5].IdProducto, IdAlmacen= alm["MZA"],  Tipo="IN", Motivo=1, Cantidad=d[5].CantidadRecibida, CostoUnitario=d[5].CostoUnitario, IdOrdenCompraDetalle=d[5].IdOrdenCompraDetalle, FechaMovimiento=DateTime.UtcNow.AddDays(-3), Referencia=$"Recepción OC {oc3.NumeroOc}" }
                };
                ctx.MovimientoInventario.AddRange(movs);
                await ctx.SaveChangesAsync();
            }
        }
    }
}