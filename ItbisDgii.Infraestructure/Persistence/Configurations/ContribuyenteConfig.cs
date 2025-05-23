using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItbisDgii.Infraestructure.Persistence.Configurations
{
    public class ContribuyenteConfig : IEntityTypeConfiguration<Contribuyente>
    {
        public void Configure(EntityTypeBuilder<Contribuyente> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.RncCedula).HasMaxLength(11).IsRequired();
            builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Tipo).IsRequired();
            builder.Property(e => e.Estatus).IsRequired();
            builder.HasIndex(e => e.RncCedula).IsUnique();

            // Relación uno a muchos con ComprobanteFiscal
            builder.HasMany(e => e.ComprobantesFiscales)
                   .WithOne(e => e.Contribuyente)
                   .HasForeignKey(e => e.ContribuyenteId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Seed de datos iniciales
            builder.HasData(
                new
                {
                    Id = Guid.Parse("67dac684-bee5-4b0a-9e48-30e3f95f7471"),
                    RncCedula = "98754321012",
                    Nombre = "JUAN PEREZ",
                    Tipo = TipoContribuyente.PersonaFisica,
                    Estatus = EstatusContribuyente.Activo,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("d991f903-4396-4aba-9fee-91fc110ab8bc"),
                    RncCedula = "123456789",
                    Nombre = "FARMACIA TU SALUD",
                    Tipo = TipoContribuyente.PersonaJuridica,
                    Estatus = EstatusContribuyente.Inactivo,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                }
            );
        }
    }
}