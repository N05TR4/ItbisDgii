using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace ItbisDgii.Infraestructure.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed default basic user
            var defaultUser = new ApplicationUser
            {
                UserName = "userBasic",
                Email = "userBasic@mail.com",
                Nombre = "Jose",
                Apellido = "Lorenzo",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            // Verificar si el usuario ya existe por email
            var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);
            if (existingUser == null)
            {
                // Crear el usuario primero con una contraseña que cumple los requisitos
                // Contraseña: Basic123! (tiene mayúscula, minúscula, número y carácter especial)
                var createResult = await userManager.CreateAsync(defaultUser, "Basic123!");

                if (createResult.Succeeded)
                {
                    // Después agregar el rol
                    var addRoleResult = await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());

                    if (!addRoleResult.Succeeded)
                    {
                        throw new Exception($"Error adding Basic role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    throw new Exception($"Error creating basic user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}