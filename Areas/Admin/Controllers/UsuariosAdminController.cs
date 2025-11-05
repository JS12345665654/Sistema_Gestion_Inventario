using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Inventario.Data;
using Sistema_Gestion_Inventario.Models;

namespace Sistema_Gestion_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    [Route("Admin/Usuarios")]
    public class UsuariosAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsuariosAdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        private bool IsAjax => Request?.Headers != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        public class UserEditVM
        {
            public string Id { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Nombre { get; set; }
            public string? Apellido { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsUsuario { get; set; }
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                usersQuery = usersQuery.Where(u => EF.Functions.Like(u.Email ?? u.UserName, $"%{q}%"));
            }

            var users = await usersQuery.ToListAsync();
            var list = new List<UserRoleVM>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                list.Add(new UserRoleVM
                {
                    Id = u.Id,
                    Email = u.Email ?? u.UserName ?? "(sin email)",
                    IsAdmin = roles.Contains("Administrador"),
                    IsUsuario = roles.Contains("Usuario")
                });
            }

            ViewData["q"] = q;
            return View(list);
        }

        [HttpGet("Detalles/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);

            var vm = new UserEditVM
            {
                Id = u.Id,
                Email = u.Email,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                IsAdmin = roles.Contains("Administrador"),
                IsUsuario = roles.Contains("Usuario")
            };

            return View("Details", vm);
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);

            var vm = new UserEditVM
            {
                Id = u.Id,
                Email = u.Email,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                IsAdmin = roles.Contains("Administrador"),
                IsUsuario = roles.Contains("Usuario")
            };

            return View("Edit", vm);
        }

        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditVM vm)
        {
            if (id != vm.Id) return NotFound();

            var u = await _userManager.FindByIdAsync(id);
            if (u is null) return NotFound();

            u.Email = vm.Email;
            u.UserName = vm.Email;
            u.Nombre = vm.Nombre;
            u.Apellido = vm.Apellido;

            var resUpdate = await _userManager.UpdateAsync(u);
            if (!resUpdate.Succeeded)
            {
                foreach (var e in resUpdate.Errors) ModelState.AddModelError("", e.Description);
                return View("Edit", vm);
            }

            // Asegurar existencia de roles
            if (!await _roleManager.RoleExistsAsync("Administrador"))
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));
            if (!await _roleManager.RoleExistsAsync("Usuario"))
                await _roleManager.CreateAsync(new IdentityRole("Usuario"));

            var current = await _userManager.GetRolesAsync(u);
            var wantAdmin = vm.IsAdmin;
            var wantUsuario = vm.IsUsuario;

            if (current.Contains("Administrador") && !wantAdmin)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Administrador");
                if (admins.Count <= 1)
                {
                    ModelState.AddModelError("", "No podés quitar el rol Administrador: es el único Administrador.");
                    return View("Edit", vm);
                }
            }

            if (wantAdmin && !current.Contains("Administrador"))
                await _userManager.AddToRoleAsync(u, "Administrador");
            if (!wantAdmin && current.Contains("Administrador"))
                await _userManager.RemoveFromRoleAsync(u, "Administrador");

            if (wantUsuario && !current.Contains("Usuario"))
                await _userManager.AddToRoleAsync(u, "Usuario");
            if (!wantUsuario && current.Contains("Usuario"))
                await _userManager.RemoveFromRoleAsync(u, "Usuario");

            TempData["toastType"] = "success";
            TempData["toastMsg"] = "Usuario actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("AgregarAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (!await _roleManager.RoleExistsAsync("Administrador"))
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));

            var res = await _userManager.AddToRoleAsync(user, "Administrador");

            if (IsAjax) return Json(new { ok = res.Succeeded, error = res.Succeeded ? null : string.Join(" | ", res.Errors.Select(e => e.Description)) });

            TempData["toastType"] = res.Succeeded ? "success" : "error";
            TempData["toastMsg"] = res.Succeeded ? $"Se otorgó Admin a {user.Email}" : string.Join(" | ", res.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("QuitarAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Administrador"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Administrador");
                if (admins.Count <= 1)
                {
                    if (IsAjax) return Json(new { ok = false, error = "No se puede quitar: es el único Administrador." });
                    TempData["toastType"] = "error";
                    TempData["toastMsg"] = "No se puede quitar: es el único Administrador.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var res = await _userManager.RemoveFromRoleAsync(user, "Administrador");

            if (IsAjax) return Json(new { ok = res.Succeeded, error = res.Succeeded ? null : string.Join(" | ", res.Errors.Select(e => e.Description)) });

            TempData["toastType"] = res.Succeeded ? "success" : "error";
            TempData["toastMsg"] = res.Succeeded ? $"Se quitó Admin a {user.Email}" : string.Join(" | ", res.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("AgregarUsuario"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (!await _roleManager.RoleExistsAsync("Usuario"))
                await _roleManager.CreateAsync(new IdentityRole("Usuario"));

            var res = await _userManager.AddToRoleAsync(user, "Usuario");

            if (IsAjax) return Json(new { ok = res.Succeeded, error = res.Succeeded ? null : string.Join(" | ", res.Errors.Select(e => e.Description)) });

            TempData["toastType"] = res.Succeeded ? "success" : "error";
            TempData["toastMsg"] = res.Succeeded ? $"Se otorgó rol Usuario a {user.Email}" : string.Join(" | ", res.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("QuitarUsuario"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var res = await _userManager.RemoveFromRoleAsync(user, "Usuario");

            if (IsAjax) return Json(new { ok = res.Succeeded, error = res.Succeeded ? null : string.Join(" | ", res.Errors.Select(e => e.Description)) });

            TempData["toastType"] = res.Succeeded ? "success" : "error";
            TempData["toastMsg"] = res.Succeeded ? $"Se quitó rol Usuario a {user.Email}" : string.Join(" | ", res.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Eliminar/{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (IsAjax) return PartialView("Delete", user);
            return View("Delete", user);
        }

        [HttpPost("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar([FromForm] string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                if (IsAjax) return Json(new { ok = false, error = "Usuario no encontrado." });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (string.Equals(currentUserId, user.Id, StringComparison.Ordinal))
            {
                if (IsAjax) return Json(new { ok = false, error = "No podés eliminar tu propio usuario." });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = "No podés eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Administrador"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Administrador");
                if (admins.Count <= 1)
                {
                    if (IsAjax) return Json(new { ok = false, error = "No se puede eliminar: es el único Administrador." });
                    TempData["toastType"] = "error";
                    TempData["toastMsg"] = "No se puede eliminar: es el único Administrador.";
                    return RedirectToAction(nameof(Index));
                }
            }

            try
            {
                await _context.MovimientoInventario
                    .Where(m => m.IdUsuarioOperador == user.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.IdUsuarioOperador, (string?)null));

                var res = await _userManager.DeleteAsync(user);

                if (IsAjax)
                    return Json(new { ok = res.Succeeded, error = res.Succeeded ? null : string.Join(" | ", res.Errors.Select(e => e.Description)) });

                TempData["toastType"] = res.Succeeded ? "success" : "error";
                TempData["toastMsg"] = res.Succeeded ? "Usuario eliminado." : string.Join(" | ", res.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                if (IsAjax) return Json(new { ok = false, error = ex.Message });
                TempData["toastType"] = "error";
                TempData["toastMsg"] = $"No se pudo eliminar: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ExportarPDF")]
        public async Task<FileResult> ExportarPDF(string? q)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                usersQuery = usersQuery.Where(u => EF.Functions.Like(u.Email ?? u.UserName, $"%{q}%"));
            }
            var users = await usersQuery.ToListAsync();

            var rows = new List<(string Email, string Roles)>();
            foreach (var u in users)
            {
                var r = await _userManager.GetRolesAsync(u);
                rows.Add((u.Email ?? u.UserName ?? "(sin email)", string.Join(", ", r)));
            }

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(25);
                    p.DefaultTextStyle(x => x.FontSize(10));
                    p.Header().Text("Usuarios y Roles").SemiBold().FontSize(16).AlignCenter();
                    p.Content().Table(t =>
                    {
                        t.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(H).Text("Email");
                            h.Cell().Element(H).Text("Roles");
                        });

                        foreach (var r in rows)
                        {
                            t.Cell().Element(B).Text(r.Email);
                            t.Cell().Element(B).Text(string.IsNullOrWhiteSpace(r.Roles) ? "-" : r.Roles);
                        }

                        static IContainer H(IContainer c) => c.Padding(4).Background(Colors.Grey.Lighten3).Border(1);
                        static IContainer B(IContainer c) => c.Padding(4).BorderBottom(1);
                    });
                    p.Footer().AlignRight().Text(System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", $"Usuarios_{System.DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }
    }
}