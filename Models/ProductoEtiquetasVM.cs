using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema_Gestion_Inventario.Models
{
    public class ProductoEtiquetasVM
    {
        public int IdProducto { get; set; }
        public string ProductoTitulo { get; set; } = "";
        public List<int> EtiquetasSeleccionadas { get; set; } = new();
        public List<SelectListItem> TodasLasEtiquetas { get; set; } = new();
    }
}
