using ItbisDgii.Infraestructure.Identity;
using ItbisDgii.Infraestructure.Persistence;
using ItbisDgii.Application;
using ItbisDgii.API.Extensions;
using ItbisDgii.Infraestructure.Identity.Context;
using ItbisDgii.Infraestructure.Identity.Models;
using ItbisDgii.Infraestructure.Identity.Seeds;
using ItbisDgii.Infraestructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddPersisrtenceInfraestructure(builder.Configuration);
builder.Services.AddIdentityInfraestructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerExtensions();
builder.Services.AddCorsExtensions();

var app = builder.Build();

// Seeding de la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Aplicar migraciones primero
        var context = services.GetRequiredService<ApplicationDbContext>();
        var identityContext = services.GetRequiredService<IdentityContext>();

        await context.Database.MigrateAsync();
        await identityContext.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed roles primero, luego usuarios
        await DefaultRoles.SeedAsync(userManager, roleManager);
        await DefaultAdminUser.SeedAsync(userManager, roleManager);
        await DefaultBasicUser.SeedAsync(userManager, roleManager);

        logger.LogInformation("Database migrated and seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database: {Message}", ex.Message);
        // En desarrollo, puedes hacer throw para ver el error completo
        if (app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Middleware de manejo de errores debe ir primero
app.UseErrorHandlingMiddleware();

// CORS
app.UseCors("AllowLocalhost");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
