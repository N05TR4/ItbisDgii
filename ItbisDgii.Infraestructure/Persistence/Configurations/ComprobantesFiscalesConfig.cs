using ItbisDgii.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItbisDgii.Infraestructure.Persistence.Configurations
{
    public class ComprobantesFiscalesConfig : IEntityTypeConfiguration<ComprobanteFiscal>
    {
        public void Configure(EntityTypeBuilder<ComprobanteFiscal> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.RncCedula).HasMaxLength(11).IsRequired();
            builder.Property(e => e.NCF).HasMaxLength(19).IsRequired();
            builder.Property(e => e.Monto).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(e => e.Itbis18).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasIndex(e => new { e.RncCedula, e.NCF }).IsUnique();

            // Seed de datos iniciales usando objetos anónimos
            builder.HasData(
                new
                {
                    Id = Guid.Parse("5d6b18c6-51e9-4a7f-aedb-6b3649abf69c"),
                    ContribuyenteId = Guid.Parse("67dac684-bee5-4b0a-9e48-30e3f95f7471"),
                    RncCedula = "98754321012",
                    NCF = "E310000000001",
                    Monto = 200.00m,
                    Itbis18 = 36.00m, // 18% de 200
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("3ee9c3fa-a3f6-4a59-b5b3-dd7fc5f623ff"),
                    ContribuyenteId = Guid.Parse("67dac684-bee5-4b0a-9e48-30e3f95f7471"),
                    RncCedula = "98754321012",
                    NCF = "E310000000002",
                    Monto = 1000.00m,
                    Itbis18 = 180.00m, // 18% de 1000
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("aa5a5c5c-5a9a-4a7f-9d2a-6b3649abf69c"),
                    ContribuyenteId = Guid.Parse("d991f903-4396-4aba-9fee-91fc110ab8bc"),
                    RncCedula = "123456789",
                    NCF = "E310000000003",
                    Monto = 500.00m,
                    Itbis18 = 90.00m, // 18% de 500
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                }
            );
        }
    }
}