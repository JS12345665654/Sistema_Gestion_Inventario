using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Inventario.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Almacen",
                columns: table => new
                {
                    IdAlmacen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacen", x => x.IdAlmacen);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CUIT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LeadTimeDias = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.IdProveedor);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompra",
                columns: table => new
                {
                    IdOrdenCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOC = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaEsperada = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    Estado = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Moneda = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "ARS"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdAlmacenRecepcion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompra", x => x.IdOrdenCompra);
                    table.CheckConstraint("CK_OrdenCompra_Estado", "[Estado] IN (0,1,2,3,4)");
                    table.CheckConstraint("CK_OrdenCompra_Moneda", "[Moneda] IN ('ARS','USD','EUR')");
                    table.ForeignKey(
                        name: "FK_OrdenCompra_AlmacenRecepcion",
                        column: x => x.IdAlmacenRecepcion,
                        principalTable: "Almacen",
                        principalColumn: "IdAlmacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Proveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "IdProveedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    IdProveedorPrincipal = table.Column<int>(type: "int", nullable: true),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CostoStd = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    PrecioSugerido = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CodigoBarras = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Producto_Categoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categoria",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Producto_ProveedorPrincipal",
                        column: x => x.IdProveedorPrincipal,
                        principalTable: "Proveedor",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraDetalle",
                columns: table => new
                {
                    IdOrdenCompraDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenCompra = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    CantidadPedida = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImpuestoPct = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DescuentoPct = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraDetalle", x => x.IdOrdenCompraDetalle);
                    table.CheckConstraint("CK_OCDet_Cantidades", "(CantidadPedida > 0 AND CantidadRecibida >= 0)");
                    table.CheckConstraint("CK_OCDet_Costos", "(CostoUnitario >= 0)");
                    table.CheckConstraint("CK_OCDet_Porcentajes", "((ImpuestoPct IS NULL OR (ImpuestoPct >= 0 AND ImpuestoPct <= 100)) AND (DescuentoPct IS NULL OR (DescuentoPct >= 0 AND DescuentoPct <= 100)))");
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_OC",
                        column: x => x.IdOrdenCompra,
                        principalTable: "OrdenCompra",
                        principalColumn: "IdOrdenCompra",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoInventario",
                columns: table => new
                {
                    IdMovimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdAlmacen = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Motivo = table.Column<short>(type: "smallint", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IdOrdenCompraDetalle = table.Column<int>(type: "int", nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Referencia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    IdUsuarioOperador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoInventario", x => x.IdMovimiento);
                    table.CheckConstraint("CK_MovInv_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_MovInv_CompraRequiere_IN_y_OCDet", "(Motivo <> 1 OR (Tipo = 'IN' AND IdOrdenCompraDetalle IS NOT NULL))");
                    table.CheckConstraint("CK_MovInv_CostoNoNeg", "(CostoUnitario IS NULL OR CostoUnitario >= 0)");
                    table.CheckConstraint("CK_MovInv_Motivo", "[Motivo] IN (1,2,3,4,5,6,7,8)");
                    table.CheckConstraint("CK_MovInv_Tipo", "[Tipo] IN ('IN','OUT')");
                    table.ForeignKey(
                        name: "FK_MovInv_Almacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacen",
                        principalColumn: "IdAlmacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovInv_OCDet",
                        column: x => x.IdOrdenCompraDetalle,
                        principalTable: "OrdenCompraDetalle",
                        principalColumn: "IdOrdenCompraDetalle",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovInv_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovInv_Usuario",
                        column: x => x.IdUsuarioOperador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Almacen_Codigo",
                table: "Almacen",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Categoria_Nombre",
                table: "Categoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdAlmacen",
                table: "MovimientoInventario",
                column: "IdAlmacen");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdOrdenCompraDetalle",
                table: "MovimientoInventario",
                column: "IdOrdenCompraDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdUsuarioOperador",
                table: "MovimientoInventario",
                column: "IdUsuarioOperador");

            migrationBuilder.CreateIndex(
                name: "IX_MovInv_FechaMovimiento",
                table: "MovimientoInventario",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovInv_ProductoAlmacen",
                table: "MovimientoInventario",
                columns: new[] { "IdProducto", "IdAlmacen" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_IdAlmacenRecepcion",
                table: "OrdenCompra",
                column: "IdAlmacenRecepcion");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_IdProveedor",
                table: "OrdenCompra",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "UQ_OrdenCompra_NumeroOC",
                table: "OrdenCompra",
                column: "NumeroOC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OCDet_IdOrdenCompra",
                table: "OrdenCompraDetalle",
                column: "IdOrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_OCDet_IdProducto",
                table: "OrdenCompraDetalle",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdCategoria",
                table: "Producto",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdProveedorPrincipal",
                table: "Producto",
                column: "IdProveedorPrincipal");

            migrationBuilder.CreateIndex(
                name: "UQ_Producto_SKU",
                table: "Producto",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Producto_CodigoBarras_NotNull",
                table: "Producto",
                column: "CodigoBarras",
                unique: true,
                filter: "[CodigoBarras] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Proveedor_CUIT",
                table: "Proveedor",
                column: "CUIT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Proveedor_Email_NotNull",
                table: "Proveedor",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "MovimientoInventario");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "OrdenCompraDetalle");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "OrdenCompra");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Almacen");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Proveedor");
        }
    }
}
