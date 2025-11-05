using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Inventario.Models
{
    public class Register
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}