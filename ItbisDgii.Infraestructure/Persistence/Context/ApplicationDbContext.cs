using ItbisDgii.Domain.Common;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Infraestructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ItbisDgii.Infraestructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Contribuyente> Contribuyentes { get; set; }
        public DbSet<ComprobanteFiscal> ComprobantesFiscales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar todas las configuraciones
            modelBuilder.ApplyConfiguration(new ContribuyenteConfig());
            modelBuilder.ApplyConfiguration(new ComprobantesFiscalesConfig());

            base.OnModelCreating(modelBuilder);
        }



        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        typeof(BaseEntity).GetProperty(nameof(BaseEntity.CreatedAt))!
                            .SetValue(entry.Entity, DateTime.UtcNow);
                        break;
                    case EntityState.Modified:
                        typeof(BaseEntity).GetProperty(nameof(BaseEntity.UpdatedAt))!
                            .SetValue(entry.Entity, DateTime.UtcNow);
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
