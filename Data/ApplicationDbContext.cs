using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Gestion_Inventario.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Categoria> Categoria { get; set; } = default!;
        public DbSet<Proveedor> Proveedor { get; set; } = default!;
        public DbSet<Almacen> Almacen { get; set; } = default!;
        public DbSet<Producto> Producto { get; set; } = default!;
        public DbSet<OrdenCompra> OrdenCompra { get; set; } = default!;
        public DbSet<OrdenCompraDetalle> OrdenCompraDetalle { get; set; } = default!;
        public DbSet<MovimientoInventario> MovimientoInventario { get; set; } = default!;
        public DbSet<Etiqueta> Etiqueta { get; set; } = default!;
        public DbSet<ProductoEtiqueta> ProductoEtiqueta { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

            // ===== CATEGORIA
            model.Entity<Categoria>(e =>
            {
                e.ToTable("Categoria");
                e.HasKey(x => x.IdCategoria);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                e.Property(x => x.Descripcion).HasMaxLength(400);
                e.Property(x => x.Activo).HasDefaultValue(true);
                e.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UQ_Categoria_Nombre");
            });

            // ===== PROVEEDOR
            model.Entity<Proveedor>(e =>
            {
                e.ToTable("Proveedor");
                e.HasKey(x => x.IdProveedor);
                e.Property(x => x.RazonSocial).IsRequired().HasMaxLength(150);
                e.Property(x => x.Cuit).IsRequired().HasMaxLength(20).HasColumnName("CUIT");
                e.Property(x => x.Email).HasMaxLength(150);
                e.Property(x => x.Telefono).HasMaxLength(40);
                e.Property(x => x.Direccion).HasMaxLength(200);
                e.Property(x => x.Ciudad).HasMaxLength(100);
                e.Property(x => x.Provincia).HasMaxLength(100);
                e.Property(x => x.Cp).HasMaxLength(15).HasColumnName("CP");
                e.Property(x => x.Activo).HasDefaultValue(true);
                e.Property(x => x.LeadTimeDias).HasDefaultValue(0);

                e.HasIndex(x => x.Cuit).IsUnique().HasDatabaseName("UQ_Proveedor_CUIT");
                e.HasIndex(x => x.Email).IsUnique()
                    .HasFilter("[Email] IS NOT NULL")
                    .HasDatabaseName("UX_Proveedor_Email_NotNull");
            });

            // ===== ALMACEN
            model.Entity<Almacen>(e =>
            {
                e.ToTable("Almacen");
                e.HasKey(x => x.IdAlmacen);
                e.Property(x => x.Codigo).IsRequired().HasMaxLength(20);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                e.Property(x => x.Direccion).HasMaxLength(200);
                e.Property(x => x.Activo).HasDefaultValue(true);
                e.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("UQ_Almacen_Codigo");
            });

            // ===== PRODUCTO
            model.Entity<Producto>(e =>
            {
                e.ToTable("Producto");
                e.HasKey(x => x.IdProducto);
                e.Property(x => x.Sku).IsRequired().HasMaxLength(50).HasColumnName("SKU");
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
                e.Property(x => x.UnidadMedida).IsRequired().HasMaxLength(20);
                e.Property(x => x.CostoStd).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                e.Property(x => x.PrecioSugerido).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                e.Property(x => x.CodigoBarras).HasMaxLength(50);
                e.Property(x => x.Activo).HasDefaultValue(true);

                e.Property(x => x.ImagenPath).HasMaxLength(260);
                e.Property(x => x.FichaTecnicaPath).HasMaxLength(260);

                e.HasIndex(x => x.Sku).IsUnique().HasDatabaseName("UQ_Producto_SKU");
                e.HasIndex(x => x.CodigoBarras).IsUnique()
                    .HasFilter("[CodigoBarras] IS NOT NULL")
                    .HasDatabaseName("UX_Producto_CodigoBarras_NotNull");

                e.HasOne(x => x.IdCategoriaNavigation)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(x => x.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Producto_Categoria");

                e.HasOne(x => x.IdProveedorPrincipalNavigation)
                    .WithMany()
                    .HasForeignKey(x => x.IdProveedorPrincipal)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Producto_ProveedorPrincipal");
            });

            // ===== ORDEN COMPRA
            model.Entity<OrdenCompra>(e =>
            {
                e.ToTable("OrdenCompra", tb =>
                {
                    tb.HasCheckConstraint("CK_OrdenCompra_Estado", "[Estado] IN (0,1,2,3,4)");
                    tb.HasCheckConstraint("CK_OrdenCompra_Moneda", "[Moneda] IN ('ARS','USD','EUR')");
                });

                e.HasKey(x => x.IdOrdenCompra);
                e.Property(x => x.NumeroOc).IsRequired().HasMaxLength(30).HasColumnName("NumeroOC");
                e.Property(x => x.FechaEmision).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.FechaEsperada).HasColumnType("datetime2(0)");
                e.Property(x => x.Estado).HasDefaultValue((short)0);
                e.Property(x => x.Moneda).IsRequired().HasMaxLength(3).IsFixedLength().HasColumnType("char(3)").HasDefaultValue("ARS");
                e.Property(x => x.Observaciones).HasMaxLength(500);

                e.HasIndex(x => x.NumeroOc).IsUnique().HasDatabaseName("UQ_OrdenCompra_NumeroOC");

                e.Property(x => x.IdOrdenCompra).ValueGeneratedOnAdd();

                e.Property(x => x.NumeroOc).IsRequired().HasMaxLength(30).HasColumnName("NumeroOC");

                e.HasOne(x => x.IdProveedorNavigation)
                    .WithMany(p => p.OrdenesCompra)
                    .HasForeignKey(x => x.IdProveedor)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrdenCompra_Proveedor");

                e.HasOne(x => x.IdAlmacenRecepcionNavigation)
                    .WithMany(a => a.OrdenesRecepcion)
                    .HasForeignKey(x => x.IdAlmacenRecepcion)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrdenCompra_AlmacenRecepcion");
            });

            // ===== ORDEN COMPRA DETALLE
            model.Entity<OrdenCompraDetalle>(e =>
            {
                e.ToTable("OrdenCompraDetalle", tb =>
                {
                    tb.HasCheckConstraint("CK_OCDet_Cantidades", "(CantidadPedida > 0 AND CantidadRecibida >= 0)");
                    tb.HasCheckConstraint("CK_OCDet_Costos", "(CostoUnitario >= 0)");
                    tb.HasCheckConstraint("CK_OCDet_Porcentajes",
                        "((ImpuestoPct IS NULL OR (ImpuestoPct >= 0 AND ImpuestoPct <= 100)) AND (DescuentoPct IS NULL OR (DescuentoPct >= 0 AND DescuentoPct <= 100)))");
                });

                e.HasKey(x => x.IdOrdenCompraDetalle);

                e.Property(x => x.CantidadPedida).HasColumnType("decimal(18,2)");
                e.Property(x => x.CantidadRecibida).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                e.Property(x => x.CostoUnitario).HasColumnType("decimal(18,2)");
                e.Property(x => x.ImpuestoPct).HasColumnType("decimal(5,2)");
                e.Property(x => x.DescuentoPct).HasColumnType("decimal(5,2)");

                e.HasOne(x => x.IdOrdenCompraNavigation)
                    .WithMany(o => o.Detalles)
                    .HasForeignKey(x => x.IdOrdenCompra)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrdenCompraDetalle_OC");

                e.HasOne(x => x.IdProductoNavigation)
                    .WithMany(p => p.OrdenesCompraDetalle)
                    .HasForeignKey(x => x.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrdenCompraDetalle_Producto");

                e.HasIndex(x => x.IdOrdenCompra).HasDatabaseName("IX_OCDet_IdOrdenCompra");
                e.HasIndex(x => x.IdProducto).HasDatabaseName("IX_OCDet_IdProducto");
            });

            // ===== MOVIMIENTO INVENTARIO
            model.Entity<MovimientoInventario>(e =>
            {
                e.ToTable("MovimientoInventario", tb =>
                {
                    tb.HasCheckConstraint("CK_MovInv_Tipo", "[Tipo] IN ('IN','OUT')");
                    tb.HasCheckConstraint("CK_MovInv_Motivo", "[Motivo] IN (1,2,3,4,5,6,7,8)");
                    tb.HasCheckConstraint("CK_MovInv_Cantidad", "[Cantidad] > 0");
                    tb.HasCheckConstraint("CK_MovInv_CostoNoNeg", "(CostoUnitario IS NULL OR CostoUnitario >= 0)");
                    tb.HasCheckConstraint("CK_MovInv_CompraRequiere_IN_y_OCDet",
                        "(Motivo <> 1 OR (Tipo = 'IN' AND IdOrdenCompraDetalle IS NOT NULL))");
                });

                e.HasKey(x => x.IdMovimiento);

                e.Property(x => x.Tipo).IsRequired().HasMaxLength(3);
                e.Property(x => x.Motivo);
                e.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
                e.Property(x => x.CostoUnitario).HasColumnType("decimal(18,2)");
                e.Property(x => x.FechaMovimiento).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.Referencia).HasMaxLength(60);
                e.Property(x => x.IdUsuarioOperador).HasMaxLength(450);
                e.Property(x => x.Observaciones).HasMaxLength(400);

                e.HasOne(x => x.IdProductoNavigation)
                    .WithMany(p => p.Movimientos)
                    .HasForeignKey(x => x.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MovInv_Producto");

                e.HasOne(x => x.IdAlmacenNavigation)
                    .WithMany(a => a.Movimientos)
                    .HasForeignKey(x => x.IdAlmacen)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MovInv_Almacen");

                e.HasOne(x => x.IdOrdenCompraDetalleNavigation)
                    .WithMany(d => d.Movimientos)
                    .HasForeignKey(x => x.IdOrdenCompraDetalle)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MovInv_OCDet");

                // FK a AspNetUsers (ApplicationUser)
                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuarioOperador)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MovInv_Usuario");

                e.HasIndex(x => new { x.IdProducto, x.IdAlmacen }).HasDatabaseName("IX_MovInv_ProductoAlmacen");
                e.HasIndex(x => x.FechaMovimiento).HasDatabaseName("IX_MovInv_FechaMovimiento");

                // ===== ETIQUETA
                model.Entity<Etiqueta>(e =>
                {
                    e.ToTable("Etiqueta");
                    e.HasKey(x => x.IdEtiqueta);
                    e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                    e.Property(x => x.Activo).HasDefaultValue(true);
                    e.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UQ_Etiqueta_Nombre");
                });

                // ===== PRODUCTO_ETIQUETA (M:N)
                model.Entity<ProductoEtiqueta>(e =>
                {
                    e.ToTable("ProductoEtiqueta");
                    e.HasKey(x => new { x.IdProducto, x.IdEtiqueta });

                    e.HasOne(x => x.IdProductoNavigation)
                        .WithMany(p => p.ProductoEtiquetas) // <- usa la navegación agregada
                        .HasForeignKey(x => x.IdProducto)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ProdEti_Producto");

                    e.HasOne(x => x.IdEtiquetaNavigation)
                        .WithMany(t => t.Productos)
                        .HasForeignKey(x => x.IdEtiqueta)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ProdEti_Etiqueta");
                });

            });
        }
    }

    // ================== ENTIDADES ==================

    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }

    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string RazonSocial { get; set; } = null!;
        public string Cuit { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }
        public string? Cp { get; set; }
        public bool Activo { get; set; } = true;
        public int LeadTimeDias { get; set; } = 0;

        public ICollection<Producto> ProductosPrincipales { get; set; } = new List<Producto>();
        public ICollection<OrdenCompra> OrdenesCompra { get; set; } = new List<OrdenCompra>();
    }

    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
        public ICollection<OrdenCompra> OrdenesRecepcion { get; set; } = new List<OrdenCompra>();
    }

    public class Producto
    {
        public int IdProducto { get; set; }
        public string Sku { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int IdCategoria { get; set; }
        public int? IdProveedorPrincipal { get; set; }
        public string UnidadMedida { get; set; } = null!;
        public decimal CostoStd { get; set; }
        public decimal PrecioSugerido { get; set; }

        public string? CodigoBarras { get; set; }
        public bool Activo { get; set; } = true;

        public string? ImagenPath { get; set; }
        public string? FichaTecnicaPath { get; set; }

        public Categoria IdCategoriaNavigation { get; set; } = null!;
        public Proveedor? IdProveedorPrincipalNavigation { get; set; }

        public ICollection<OrdenCompraDetalle> OrdenesCompraDetalle { get; set; } = new List<OrdenCompraDetalle>();
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();

        // *** agregado (necesario para M:N)
        public ICollection<ProductoEtiqueta> ProductoEtiquetas { get; set; } = new List<ProductoEtiqueta>();
    }

    public class OrdenCompra
    {
        public int IdOrdenCompra { get; set; }
        public string NumeroOc { get; set; } = null!;
        public int IdProveedor { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaEsperada { get; set; }
        public short Estado { get; set; }
        public string Moneda { get; set; } = "ARS";
        public string? Observaciones { get; set; }
        public int? IdAlmacenRecepcion { get; set; }

        public Proveedor IdProveedorNavigation { get; set; } = null!;
        public Almacen? IdAlmacenRecepcionNavigation { get; set; }

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }

    public class OrdenCompraDetalle
    {
        public int IdOrdenCompraDetalle { get; set; }
        public int IdOrdenCompra { get; set; }
        public int IdProducto { get; set; }
        public decimal CantidadPedida { get; set; }
        public decimal CantidadRecibida { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal? ImpuestoPct { get; set; }
        public decimal? DescuentoPct { get; set; }

        public OrdenCompra IdOrdenCompraNavigation { get; set; } = null!;
        public Producto IdProductoNavigation { get; set; } = null!;
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }

    public class MovimientoInventario
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public int IdAlmacen { get; set; }
        public string Tipo { get; set; } = null!;
        public short Motivo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? IdOrdenCompraDetalle { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? Referencia { get; set; }
        public string? IdUsuarioOperador { get; set; }
        public string? Observaciones { get; set; }

        public Producto IdProductoNavigation { get; set; } = null!;
        public Almacen IdAlmacenNavigation { get; set; } = null!;
        public OrdenCompraDetalle? IdOrdenCompraDetalleNavigation { get; set; }
    }
}