using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Inventario.Models;
using Sistema_Gestion_Inventario.Data;
using System.Security.Claims;

namespace Sistema_Gestion_Inventario.Controllers
{
    public class CuentaController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CuentaController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? HttpContext.Request.Query["ReturnUrl"].ToString();
            return View(new LoginRegister());
        }

        [HttpGet("Cuenta/Login")]
        [AllowAnonymous]
        public IActionResult LoginGet(string? returnUrl = null)
        {
            return RedirectToAction(nameof(Index), new { returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "Login")] Login model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Index", new LoginRegister { Login = model });

            var user = await _userManager.FindByEmailAsync(model.Email!);
            var result = await _signInManager.PasswordSignInAsync(
                userName: user?.UserName ?? model.Email!,
                password: model.Password!,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (user != null)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    if (!claims.Any(c => c.Type == "Nombre"))
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Nombre", user.Nombre ?? user.Email!));
                }

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            TempData["Mensaje"] = "Credenciales incorrectas.";
            return View("Index", new LoginRegister { Login = model });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([Bind(Prefix = "Register")] Register model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? HttpContext.Request.Query["ReturnUrl"].ToString();

            if (!ModelState.IsValid)
                return View("Index", new LoginRegister { Register = model });

            var exists = await _userManager.FindByEmailAsync(model.Email!);
            if (exists != null)
            {
                ModelState.AddModelError(string.Empty, "El email ya está registrado.");
                return View("Index", new LoginRegister { Register = model });
            }

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                Nombre = model.Nombre
            };

            var createRes = await _userManager.CreateAsync(usuario, model.Password!);
            if (createRes.Succeeded)
            {
                // Asegura que exista el rol "Usuario"
                if (!await _roleManager.RoleExistsAsync("Usuario"))
                    await _roleManager.CreateAsync(new IdentityRole("Usuario"));

                await _userManager.AddToRoleAsync(usuario, "Usuario");
                await _userManager.AddClaimAsync(usuario, new Claim("Nombre", usuario.Nombre ?? usuario.Email!));
                await _signInManager.SignInAsync(usuario, isPersistent: false);

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            foreach (var e in createRes.Errors)
                ModelState.AddModelError(string.Empty, e.Description);

            return View("Index", new LoginRegister { Register = model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}