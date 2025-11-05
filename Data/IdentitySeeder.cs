using Microsoft.AspNetCore.Identity;

namespace Sistema_Gestion_Inventario.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Crear roles si faltan
            foreach (var role in new[] { Roles.Administrador, Roles.Usuario })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Admin base
            var adminEmail = "joaquinsoberon@inventario.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var create = await userManager.CreateAsync(admin, "Admin1234$");
                if (!create.Succeeded)
                    throw new Exception(string.Join(" | ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.Administrador))
            {
                var add = await userManager.AddToRoleAsync(admin, Roles.Administrador);
                if (!add.Succeeded)
                    throw new Exception(string.Join(" | ", add.Errors.Select(e => e.Description)));
            }
        }
    }
}