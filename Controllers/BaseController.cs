using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sistema_Gestion_Inventario.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Limpiar propiedades de navegación del ModelState
            RemoveNavigationProperties();
            base.OnActionExecuting(context);
        }

        protected void RemoveNavigationProperties()
        {
            var keysToRemove = ModelState.Keys
                .Where(k => k.EndsWith("Navigation", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }
        }

        // Método para remover propiedades específicas
        protected void RemoveFromModelState(params string[] propertyNames)
        {
            foreach (var prop in propertyNames)
            {
                ModelState.Remove(prop);
            }
        }
    }
}