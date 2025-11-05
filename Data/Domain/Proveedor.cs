using System.ComponentModel.DataAnnotations;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class Proveedor
    {
        [Key] public int IdProveedor { get; set; }

        [Display(Name = "Razón social"), Required, MaxLength(150)]
        public string RazonSocial { get; set; } = null!;

        [Display(Name = "CUIT"), Required, MaxLength(20)]
        [CuitUnico(ErrorMessage ="Ya existe un proveedor con ese CUIT")]
        public string Cuit { get; set; } = null!;

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        [Display(Name = "Teléfono"), MaxLength(40)]
        public string? Telefono { get; set; }

        [Display(Name = "Dirección"), MaxLength(200)]
        public string? Direccion { get; set; }

        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }

        [Display(Name = "CP"), MaxLength(15)]
        public string? Cp { get; set; }

        [Display(Name = "Activo")] public bool Activo { get; set; } = true;

        [Display(Name = "Lead Time (días)")] public int LeadTimeDias { get; set; } = 0;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public ICollection<OrdenCompra> OrdenesCompra { get; set; } = new List<OrdenCompra>();
    }
}
