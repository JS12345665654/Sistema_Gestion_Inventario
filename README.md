# Sistema_Gestion_Inventario# 🛒 Sistema de Gestión de Inventario (ASP.NET Core MVC + EF Core)

Administrá productos, proveedores, almacenes y categorías; registrá Órdenes de Compra y Movimientos de Inventario; consultá el stock actual por producto/almacén mediante un Stored Procedure. Seguridad con Identity (roles) y UX moderna (DataTables, SweetAlert2).

Índice

🎯 Objetivo general

✅ Objetivos específicos

🔐 Alcance y roles

🧰 Tecnologías

📦 Paquetes NuGet (y para qué)

🖥️ Requisitos y ejecución

🗂️ Estructura mínima

🧪 Probar en 5 minutos

📈 Stock (cómo se calcula)

🌐 Cultura y localización

🧩 Troubleshooting rápido

🖼️ Manejo de imágenes y fichas técnicas

👤 Autor

💾 Hosting



🎯 Objetivo general

Gestionar inventario y compras con control de acceso por roles y exportaciones.

✅ Objetivos específicos

ABM de Productos, Proveedores, Almacenes y Categorías.

Orden de Compra (cabecera + renglones) y Movimientos de Inventario (entradas/salidas).

Stored Procedure para stock actual por producto/almacén.

Exportaciones a PDF/Excel y búsqueda sensitiva en listados.

Hardening: autorización global y CSRF en formularios.




🔐 Alcance y roles

Compartido (Usuario y Administrador):
• Crear y ver detalle de Productos, Proveedores, Almacenes y Categorías.
• Búsqueda sensitiva en listados y Exportar (PDF/Excel).
• Consulta de stock por Stored Procedure.

Solo Administrador:
• Editar y eliminar Productos/Proveedores/Almacenes/Categorías.
• CRUD de OrdenCompra y MovimientoInventario.
• Gestión de usuarios y roles.

💡 En Productos/Details se deja visible el botón Editar para demostrar restricción: sin permisos, la acción es denegada por política global.



🧰 Tecnologías

• Backend: ASP.NET Core MVC (.NET 8), EF Core (SQL Server), Identity.
• Frontend: Bootstrap, jQuery, jQuery Validate (es-ES), DataTables (es-ES), SweetAlert2, Select2.
• Export: ClosedXML (Excel), OpenXML, QuestPDF (PDF).
• BD: SQL Server, Stored Procedure de stock actual.



📦 Paquetes NuGet (y para qué)

• Microsoft.EntityFrameworkCore — ORM para acceso a datos con LINQ.
• Microsoft.EntityFrameworkCore.SqlServer — Proveedor EF Core para SQL Server.
• Microsoft.EntityFrameworkCore.Tools — Migraciones, scaffolding y comandos de diseño.
• Microsoft.AspNetCore.Identity.EntityFrameworkCore — Store de ASP.NET Identity sobre EF Core.
• Microsoft.AspNetCore.Identity.UI — Páginas/estilos base de Identity (si se usan).
• Microsoft.Data.SqlClient — Ejecución de Stored Procedures y SQL parametrizado.
• ClosedXML — Exportación a Excel (.xlsx) sin manejar bajo nivel.
• DocumentFormat.OpenXml — Soporte OpenXML (Excel/Word) cuando hace falta.
• QuestPDF — Generación de PDF para reportes/exportaciones.

👉 Dependencias front (no NuGet): Bootstrap, jQuery, DataTables, SweetAlert2, Select2 (CDN).



🖥️ Requisitos y ejecución

• .NET SDK 8.0, Visual Studio 2022 o dotnet CLI.
• SQL Server / SQL Express con la base accesible.
• Configurar ConnectionStrings:DefaultConnection en appsettings.json y ejecutar:
– dotnet restore
– dotnet run
• Al primer arranque se crean roles y (si está configurado) el usuario admin semilla.
• Publicación (opcional):
– dotnet publish -c Release -o ./publish



🗂️ Estructura mínima

• /Controllers
• /Areas/Admin/Controllers
• /Views
• /Areas/Admin/Views
• /Data (DbContext, seeding)
• /wwwroot (css, js, imágenes)
• /Models (ViewModels y validaciones)
• /docs/capturas (imágenes para la doc)



🧪 Probar en 5 minutos

Crear OC con renglones y un almacén de recepción.

En OC → Detalle → Recibir OC (solo Admin): cargar cantidades y confirmar.
– Se generan Movimientos IN por renglón y se actualiza el estado (Parcial/Recibida).

Salida de inventario (solo Admin): Inventario → Salida → Crear, elegir producto, almacén, cantidad y motivo (Venta/Consumo).
– Valida stock disponible; si excede, rechaza.

Stock actual: consultar el SP (con o sin filtros por producto/almacén).

Exportaciones: desde los listados, exportar a PDF/Excel luego de filtrar.



📈 Stock (cómo se calcula)

Para cada Producto + Almacén:
Stock actual = Σ(entradas Tipo=IN) − Σ(salidas Tipo=OUT)
El SP devuelve además la fecha del último movimiento por par.



🌐 Cultura y localización

• Cliente: validaciones y mensajes en español, números con coma (ej.: 12,34), DataTables es-ES, confirmaciones con SweetAlert2.
• Servidor: cultura es-AR y validaciones de unicidad (SKU, CódigoBarras, CUIT, Email, Nº OC) con mensajes claros.



🧩 Troubleshooting rápido

• 400 Antiforgery: agregar AntiForgeryToken en formularios POST.
• AccessDenied: faltan permisos o rol para la acción (revisar rol actual).
• Stock insuficiente: la salida intenta superar el stock disponible.
• Base de datos: revisar cadena de conexión y que el servidor esté accesible.


🖼️ Manejo de imágenes y fichas técnicas
- Entradas permitidas:

• Imagen del producto: archivo local o URL pública (mutuamente excluyentes).
• Ficha técnica (opcional): archivo PDF.

El formulario impide elegir ambas opciones a la vez (radios “Subir archivo / Usar URL”) y el servidor vuelve a validar que no vengan las dos.

✅ Validaciones del servidor

• Extensiones de imagen permitidas: .jpg, .jpeg, .png, .gif, .webp
• Extensión de ficha técnica: .pdf
• Tamaño máximo de archivo: 10 MB
• URL de imagen: debe ser http/https válido

Si algo no cumple, se rechaza con mensaje claro (por ejemplo: “Extensión no permitida” o “Archivo demasiado grande”).

🗂️ Dónde se guardan

• Imágenes: carpeta “/uploads/img” dentro de wwwroot
• Fichas técnicas: carpeta “/uploads/fichas” dentro de wwwroot
• Antes de guardar, se asegura la existencia del directorio.
• El nombre del archivo se genera con un GUID (sin guiones) + la extensión, para evitar choques de nombres y revelar lo mínimo.

Ejemplo de ruta resultante (visible por el navegador):
/ uploads / img / 3c4714dc2c6e43a0a479b9a05bedd5a.png

💾 Flujo en Crear (POST)

Se valida que venga archivo o URL, pero no ambos.

Si viene archivo, se verifica extensión/tamaño y se guarda localmente; el modelo recibe ImagenPath con la ruta relativa.

Si viene URL, se valida que sea http/https y se asigna tal cual a ImagenPath.

Si viene PDF, se guarda en “/uploads/fichas” con las mismas validaciones.

Se persiste el producto y se muestra toast de éxito.

Roles: Crear está permitido para Administrador y Usuario.

✏️ Flujo en Editar (POST)

Se repite la validación “archivo o URL”.

Si se sube un nuevo archivo de imagen:
– Se guarda con GUID en “/uploads/img”.
– Si la imagen anterior era local y pertenecía a “/uploads/img”, se borra el archivo viejo para evitar huérfanos.

Si se indica nueva URL:
– Si la imagen anterior era local en “/uploads/img”, se borra el archivo viejo.
– ImagenPath pasa a ser la URL externa.

Para la ficha técnica (PDF) se aplica el mismo criterio: al reemplazar, se borra el PDF previo solo si era local en “/uploads/fichas”.

Se guardan cambios y se muestra toast de éxito.

Roles: Editar está limitado a Administrador.

🗑️ Flujo en Eliminar

Antes de borrar el producto, si ImagenPath y/o FichaTecnicaPath apuntan a archivos locales dentro de las carpetas gestionadas, se eliminan físicamente (limpieza de huérfanos). Luego se borra el registro.

Roles: Eliminar está limitado a Administrador.

🔒 Medidas de seguridad aplicadas

• Lista blanca de extensiones + tope de tamaño.
• Las URL deben ser http/https válidas.
• Los borrados de archivos verifican que el path empiece por la carpeta gestionada antes de eliminar (no borra nada fuera de “/uploads/...”).
• Formularios con Anti-Forgery Token.
• Acceso controlado por roles (Crear: Admin/Usuario; Editar/Eliminar: Admin).
• Las rutas finales quedan bajo wwwroot, por lo que se sirven como estáticos sin pasar por controladores.

👁️‍🗨️ UX en las vistas

• En “Crear”: radios para elegir “Subir archivo” o “Usar URL”, con enable/disable de inputs, y restricción de tipos (accept) del lado cliente.
• En “Editar”: se muestra preview de la imagen actual (si existe), y el mismo selector de “archivo vs URL”.
• Select2 para combos de Categoría y Proveedor; validación de mensajes en español.

⚙️ Constantes de configuración (en el controlador)

• Extensiones imágenes: ImgExt (.jpg, .jpeg, .png, .gif, .webp)
• Extensión ficha: DocExt (.pdf)
• Tamaño máximo: MaxFileSize = 10 MB
• Carpetas: ImgFolder = “/uploads/img”, FichasFolder = “/uploads/fichas”
• Métodos clave: EnsureFolder (crea la carpeta), SaveLocalAsync (valida y guarda), DeleteLocalIfOwned (borra solo si es un archivo “propio” del sistema)


👤 Autor

Joaquín Soberon — Estudiante / Proyecto final de Programación Web.


💾 Hosting

Tanto el backend ASP.NET Core como la base SQL Server están alojados en Moonster ASP.NET (Cuenta con certificado SSL).
