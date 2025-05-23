using ItbisDgii.Application.Interfaces;
using ItbisDgii.Infraestructure.Persistence.Context;
using ItbisDgii.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ItbisDgii.Infraestructure.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddPersisrtenceInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            #region Repositories
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            #endregion
        }
    }
}
