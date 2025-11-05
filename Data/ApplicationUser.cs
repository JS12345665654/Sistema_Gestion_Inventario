using Microsoft.AspNetCore.Identity;

namespace Sistema_Gestion_Inventario.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}