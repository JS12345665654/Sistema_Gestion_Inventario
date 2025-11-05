using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_Gestion_Inventario.Data;

namespace Sistema_Gestion_Inventario.Models
{
    public class ProductoEditarVM
    {
        public Producto Producto { get; set; } = new();
        public int[] EtiquetasIds { get; set; } = Array.Empty<int>();
        public SelectList? Etiquetas { get; set; }
    }
}
