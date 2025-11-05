using System.ComponentModel.DataAnnotations;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class Almacen
    {
        [Key] public int IdAlmacen { get; set; }

        [Display(Name = "Código"), Required, MaxLength(20)]
        [CodigoAlmacenUnico(ErrorMessage = "Ya existe un almacen con este código")]
        public string Codigo { get; set; } = null!;

        [Display(Name = "Nombre"), Required, MaxLength(100)]
        [NombreAlmacenUnico(ErrorMessage = "Ya existe un almacen con ese nombre")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Dirección"), MaxLength(200)]
        public string? Direccion { get; set; }

        [Display(Name = "Activo")] public bool Activo { get; set; } = true;

        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
        public ICollection<OrdenCompra> OrdenesRecepcion { get; set; } = new List<OrdenCompra>();
    }
}
