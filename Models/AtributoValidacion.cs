using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Inventario.Data;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Inventario.Models
{
    // ==================== PRODUCTO ====================

    /// Valida que el SKU sea único en la tabla Producto
    public class SkuUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var sku = value.ToString()!.Trim();

            var producto = validationContext.ObjectInstance as Data.Domain.Producto;
            var idActual = producto?.IdProducto ?? 0;

            var existe = context.Producto.Any(p => p.Sku == sku && p.IdProducto != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un producto con este SKU.");

            return ValidationResult.Success;
        }
    }

    /// Valida que el Código de Barras sea único en la tabla Producto
    public class CodigoBarrasUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var codigoBarras = value.ToString()!.Trim();

            var producto = validationContext.ObjectInstance as Data.Domain.Producto;
            var idActual = producto?.IdProducto ?? 0;

            var existe = context.Producto.Any(p => p.CodigoBarras == codigoBarras && p.IdProducto != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un producto con este código de barras.");

            return ValidationResult.Success;
        }
    }

    // ==================== PROVEEDOR ====================

    /// Valida que el CUIT sea único en la tabla Proveedor
    public class CuitUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var cuit = value.ToString()!.Trim();

            var proveedor = validationContext.ObjectInstance as Data.Domain.Proveedor;
            var idActual = proveedor?.IdProveedor ?? 0;

            var existe = context.Proveedor.Any(p => p.Cuit == cuit && p.IdProveedor != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un proveedor con este CUIT.");

            return ValidationResult.Success;
        }
    }

    /// Valida que el Email del Proveedor sea único
    public class EmailProveedorUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var email = value.ToString()!.Trim().ToLower();

            var proveedor = validationContext.ObjectInstance as Data.Domain.Proveedor;
            var idActual = proveedor?.IdProveedor ?? 0;

            var existe = context.Proveedor.Any(p => p.Email != null && p.Email.ToLower() == email && p.IdProveedor != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un proveedor con este email.");

            return ValidationResult.Success;
        }
    }

    // ==================== CATEGORÍA ====================

    /// Valida que el nombre de Categoría sea único
    public class NombreCategoriaUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var nombre = value.ToString()!.Trim();

            var categoria = validationContext.ObjectInstance as Data.Domain.Categoria;
            var idActual = categoria?.IdCategoria ?? 0;

            var existe = context.Categoria.Any(c => c.Nombre == nombre && c.IdCategoria != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe una categoría con este nombre.");

            return ValidationResult.Success;
        }
    }

    // ==================== ALMACÉN ====================

    /// Valida que el código de Almacén sea único
    public class CodigoAlmacenUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var codigo = value.ToString()!.Trim();

            var almacen = validationContext.ObjectInstance as Data.Domain.Almacen;
            var idActual = almacen?.IdAlmacen ?? 0;

            var existe = context.Almacen.Any(a => a.Codigo == codigo && a.IdAlmacen != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un almacén con este código.");

            return ValidationResult.Success;
        }
    }

    /// Valida que el nombre de Almacén sea único
    public class NombreAlmacenUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var nombre = value.ToString()!.Trim();

            var almacen = validationContext.ObjectInstance as Data.Domain.Almacen;
            var idActual = almacen?.IdAlmacen ?? 0;

            var existe = context.Almacen.Any(a => a.Nombre == nombre && a.IdAlmacen != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe un almacén con este nombre.");

            return ValidationResult.Success;
        }
    }

    // ==================== ETIQUETA ====================

    /// Valida que el nombre de Etiqueta sea único
    public class NombreEtiquetaUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var nombre = value.ToString()!.Trim();

            var etiqueta = validationContext.ObjectInstance as Etiqueta;
            var idActual = etiqueta?.IdEtiqueta ?? 0;

            var existe = context.Etiqueta.Any(e => e.Nombre == nombre && e.IdEtiqueta != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe una etiqueta con este nombre.");

            return ValidationResult.Success;
        }
    }

    // ==================== ORDEN DE COMPRA ====================

    /// Valida que el número de OC sea único
    public class NumeroOcUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
            var numeroOc = value.ToString()!.Trim();

            var orden = validationContext.ObjectInstance as Data.Domain.OrdenCompra;
            var idActual = orden?.IdOrdenCompra ?? 0;

            var existe = context.OrdenCompra.Any(o => o.NumeroOc == numeroOc && o.IdOrdenCompra != idActual);

            if (existe)
                return new ValidationResult(ErrorMessage ?? "Ya existe una orden de compra con este número.");

            return ValidationResult.Success;
        }
    }
}