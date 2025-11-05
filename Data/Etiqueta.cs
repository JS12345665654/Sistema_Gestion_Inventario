using System.Collections.Generic;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Data
{
    public class Etiqueta
    {
        public int IdEtiqueta { get; set; }

        [NombreEtiquetaUnico(ErrorMessage = "Ya existe una etiqueta con este nombre")]
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; } = true;

        public ICollection<ProductoEtiqueta> Productos { get; set; } = new List<ProductoEtiqueta>();
    }

    public class ProductoEtiqueta
    {
        public int IdProducto { get; set; }
        public int IdEtiqueta { get; set; }

        public Producto IdProductoNavigation { get; set; } = null!;
        public Etiqueta IdEtiquetaNavigation { get; set; } = null!;
    }
}