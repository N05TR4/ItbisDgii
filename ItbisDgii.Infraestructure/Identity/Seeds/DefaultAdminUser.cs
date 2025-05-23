using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace ItbisDgii.Infraestructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed default admin user
            var defaultUser = new ApplicationUser
            {
                UserName = "userAdmin",
                Email = "userAdmin@mail.com",
                Nombre = "Jose Alberto",
                Apellido = "Vasquez Lorenzo",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            // Verificar si el usuario ya existe por email
            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);
            if (existingUser == null)
            {
                // Crear el usuario primero con una contraseña que cumple los requisitos
                // Contraseña: Admin123! (tiene mayúscula, minúscula, número y carácter especial)
                var createResult = await userManager.CreateAsync(defaultUser, "Admin123!");

                if (createResult.Succeeded)
                {
                    // Después agregar los roles
                    var addAdminRoleResult = await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    var addBasicRoleResult = await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());

                    if (!addAdminRoleResult.Succeeded)
                    {
                        throw new Exception($"Error adding Admin role: {string.Join(", ", addAdminRoleResult.Errors.Select(e => e.Description))}");
                    }

                    if (!addBasicRoleResult.Succeeded)
                    {
                        throw new Exception($"Error adding Basic role: {string.Join(", ", addBasicRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    throw new Exception($"Error creating admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}