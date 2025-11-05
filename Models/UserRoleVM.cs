namespace Sistema_Gestion_Inventario.Models
{
    public class UserRoleVM
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsUsuario { get; set; }
    }
}
