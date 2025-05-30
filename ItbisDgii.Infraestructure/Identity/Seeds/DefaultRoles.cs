﻿using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace ItbisDgii.Infraestructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));


        }
    }
}
