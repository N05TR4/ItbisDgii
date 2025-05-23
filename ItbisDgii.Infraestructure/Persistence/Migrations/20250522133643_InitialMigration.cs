using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItbisDgii.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contribuyentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RncCedula = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribuyentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesFiscales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContribuyenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RncCedula = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    NCF = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Itbis18 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesFiscales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprobantesFiscales_Contribuyentes_ContribuyenteId",
                        column: x => x.ContribuyenteId,
                        principalTable: "Contribuyentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Contribuyentes",
                columns: new[] { "Id", "CreatedAt", "Estatus", "Nombre", "RncCedula", "Tipo", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("67dac684-bee5-4b0a-9e48-30e3f95f7471"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "JUAN PEREZ", "98754321012", 0, null },
                    { new Guid("d991f903-4396-4aba-9fee-91fc110ab8bc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "FARMACIA TU SALUD", "123456789", 1, null }
                });

            migrationBuilder.InsertData(
                table: "ComprobantesFiscales",
                columns: new[] { "Id", "ContribuyenteId", "CreatedAt", "Itbis18", "Monto", "NCF", "RncCedula", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3ee9c3fa-a3f6-4a59-b5b3-dd7fc5f623ff"), new Guid("67dac684-bee5-4b0a-9e48-30e3f95f7471"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 180.00m, 1000.00m, "E310000000002", "98754321012", null },
                    { new Guid("5d6b18c6-51e9-4a7f-aedb-6b3649abf69c"), new Guid("67dac684-bee5-4b0a-9e48-30e3f95f7471"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 36.00m, 200.00m, "E310000000001", "98754321012", null },
                    { new Guid("aa5a5c5c-5a9a-4a7f-9d2a-6b3649abf69c"), new Guid("d991f903-4396-4aba-9fee-91fc110ab8bc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 90.00m, 500.00m, "E310000000003", "123456789", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesFiscales_ContribuyenteId",
                table: "ComprobantesFiscales",
                column: "ContribuyenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesFiscales_RncCedula_NCF",
                table: "ComprobantesFiscales",
                columns: new[] { "RncCedula", "NCF" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contribuyentes_RncCedula",
                table: "Contribuyentes",
                column: "RncCedula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComprobantesFiscales");

            migrationBuilder.DropTable(
                name: "Contribuyentes");
        }
    }
}
