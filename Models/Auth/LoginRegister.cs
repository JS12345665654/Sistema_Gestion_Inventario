namespace Sistema_Gestion_Inventario.Models
{
    public class LoginRegister
    {
        public Login Login { get; set; } = new();
        public Register Register { get; set; } = new();
    }
}