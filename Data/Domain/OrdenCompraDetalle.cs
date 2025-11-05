using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class OrdenCompraDetalle
    {
        [Key] public int IdOrdenCompraDetalle { get; set; }

        [Display(Name = "Orden de compra")] public int IdOrdenCompra { get; set; }
        [Display(Name = "Producto")] public int IdProducto { get; set; }

        [Display(Name = "Cant. pedida")] public decimal CantidadPedida { get; set; }
        [Display(Name = "Cant. recibida")] public decimal CantidadRecibida { get; set; }
        [Display(Name = "Costo unitario"), DataType(DataType.Currency)] public decimal CostoUnitario { get; set; }
        [Display(Name = "% Impuesto")] public decimal? ImpuestoPct { get; set; }
        [Display(Name = "% Descuento")] public decimal? DescuentoPct { get; set; }

        [Display(Name = "Orden de compra")] public OrdenCompra? IdOrdenCompraNavigation { get; set; }
        [Display(Name = "Producto")] public Producto? IdProductoNavigation { get; set; }

        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }
}
