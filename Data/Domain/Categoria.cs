using System.ComponentModel.DataAnnotations;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data.Domain
{
    public class Categoria
    {
        [Key] public int IdCategoria { get; set; }

        [Display(Name = "Categoría"), Required, MaxLength(100)]
        [NombreCategoriaUnico(ErrorMessage = "Ya existe una categoria con ese nombre")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Descripción"), MaxLength(400)]
        [Required]
        public string? Descripcion { get; set; }

        [Display(Name = "Activa")]
        public bool Activo { get; set; } = true;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
