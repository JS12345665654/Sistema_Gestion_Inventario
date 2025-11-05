using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class MovimientoInventario
    {
        [Key]
        public int IdMovimiento { get; set; }

        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [Display(Name = "Almacén")]
        public int IdAlmacen { get; set; }

        [Display(Name = "Tipo"), MaxLength(3)]
        public string Tipo { get; set; } = "IN";

        [Display(Name = "Motivo")]
        public short Motivo { get; set; }

        [Display(Name = "Cantidad")]
        public decimal Cantidad { get; set; }

        [Display(Name = "Costo unit."), DataType(DataType.Currency)]
        public decimal? CostoUnitario { get; set; }

        [Display(Name = "OC Detalle")]
        public int? IdOrdenCompraDetalle { get; set; }

        [Display(Name = "Fecha")]
        public DateTime FechaMovimiento { get; set; }

        [MaxLength(60)]
        public string? Referencia { get; set; }

        [MaxLength(450)]
        public string? IdUsuarioOperador { get; set; }

        [MaxLength(400)]
        public string? Observaciones { get; set; }
        public Producto IdProductoNavigation { get; set; } = null!;
        public Almacen IdAlmacenNavigation { get; set; } = null!;
        public OrdenCompraDetalle? IdOrdenCompraDetalleNavigation { get; set; }
    }
}
