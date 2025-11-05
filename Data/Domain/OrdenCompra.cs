using System.ComponentModel.DataAnnotations;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class OrdenCompra
    {
        [Key] public int IdOrdenCompra { get; set; }

        [Display(Name = "N° OC"), Required, MaxLength(30)]
        [NumeroOcUnico(ErrorMessage = "Ya existe un orden de compras con este número")]
        public string NumeroOc { get; set; } = null!;

        [Display(Name = "Proveedor")] public int IdProveedor { get; set; }

        [Display(Name = "F. Emisión"), DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Display(Name = "F. Esperada"), DataType(DataType.Date)]
        public DateTime? FechaEsperada { get; set; }

        [Display(Name = "Estado")] public short Estado { get; set; }  // 0..4

        [Display(Name = "Moneda"), MaxLength(3)]
        public string Moneda { get; set; } = "ARS";

        [Display(Name = "Observaciones"), MaxLength(500)]
        public string? Observaciones { get; set; }

        [Display(Name = "Almacén recepción")] public int? IdAlmacenRecepcion { get; set; }

        [Display(Name = "Proveedor")] public Proveedor? IdProveedorNavigation { get; set; }
        [Display(Name = "Almacén recepción")] public Almacen? IdAlmacenRecepcionNavigation { get; set; }

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }
}
