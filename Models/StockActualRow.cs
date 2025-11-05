namespace Sistema_Gestion_Inventario.Models
{
    public class StockActualRow
    {
        public int IdProducto { get; set; }
        public string Sku { get; set; } = "";
        public string Producto { get; set; } = "";
        public int IdAlmacen { get; set; }
        public string AlmacenCodigo { get; set; } = "";
        public string Almacen { get; set; } = "";
        public decimal StockActual { get; set; }
        public DateTime? UltimoMovimiento { get; set; }
    }
}
