using System.ComponentModel.DataAnnotations;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class Producto
    {
        [Key] public int IdProducto { get; set; }

        [Display(Name = "SKU"), Required, MaxLength(50)]
        [SkuUnico(ErrorMessage = "Ya existe un producto con ese SKU")]
        public string Sku { get; set; } = null!;

        [Display(Name = "Nombre"), Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Categoría")] public int IdCategoria { get; set; }

        [Display(Name = "Proveedor principal")] public int? IdProveedorPrincipal { get; set; }

        [Display(Name = "Unidad de medida"), Required, MaxLength(20)]
        public string UnidadMedida { get; set; } = null!;

        [Display(Name = "Costo estándar"), DataType(DataType.Currency)]
        public decimal CostoStd { get; set; }

        [Display(Name = "Precio sugerido"), DataType(DataType.Currency)]
        public decimal PrecioSugerido { get; set; }

        public string? ImagenPath { get; set; }

        public string? FichaTecnicaPath { get; set; }

        [Display(Name = "Código de barras"), MaxLength(50)]
        [CodigoBarrasUnico(ErrorMessage = "El código de barras no puede ser igual al de un producto ya existente")]
        public string? CodigoBarras { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Categoría")]
        public Categoria? IdCategoriaNavigation { get; set; }
        [Display(Name = "Proveedor")]
        public Proveedor? IdProveedorPrincipalNavigation { get; set; }

        public ICollection<OrdenCompraDetalle> OrdenesCompraDetalle { get; set; } = new List<OrdenCompraDetalle>();
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();

        public ICollection<ProductoEtiqueta> ProductoEtiquetas { get; set; } = new List<ProductoEtiqueta>();

    }
}
