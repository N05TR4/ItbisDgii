using FluentAssertions;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Persistence.Context;
using ItbisDgii.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ItbisDgii.Tests.Infraestructure.Persistence.Repositories
{
    public class ComprobanteFiscalRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ComprobanteFiscalRepository _repository;

        public ComprobanteFiscalRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ComprobanteFiscalRepository(_context);

            // Seed initial data
            SeedData();
        }

        private void SeedData()
        {
            var contribuyente1 = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);
            var contribuyente2 = new Contribuyente("123456789", "Farmacia ABC", TipoContribuyente.PersonaJuridica, EstatusContribuyente.Activo);

            _context.Contribuyentes.AddRange(contribuyente1, contribuyente2);
            _context.SaveChanges();

            var comprobante1 = new ComprobanteFiscal("12345678901", "E310000000001", 100m);
            var comprobante2 = new ComprobanteFiscal("12345678901", "E310000000002", 200m);
            var comprobante3 = new ComprobanteFiscal("123456789", "E310000000003", 500m);

            comprobante1.ContribuyenteId = contribuyente1.Id;
            comprobante2.ContribuyenteId = contribuyente1.Id;
            comprobante3.ContribuyenteId = contribuyente2.Id;

            _context.ComprobantesFiscales.AddRange(comprobante1, comprobante2, comprobante3);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByRncCedulaAsync_ConRncExistente_DeberiaRetornarComprobantesFiscales()
        {
            // Act
            var result = await _repository.GetByRncCedulaAsync("12345678901");

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(c => c.RncCedula.Should().Be("12345678901"));
        }

        [Fact]
        public async Task GetByRncCedulaAsync_ConRncInexistente_DeberiaRetornarListaVacia()
        {
            // Act
            var result = await _repository.GetByRncCedulaAsync("99999999999");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddAsync_ConNuevoComprobante_DeberiaAgregarCorrectamente()
        {
            // Arrange
            var nuevoComprobante = new ComprobanteFiscal("123456789", "E310000000004", 300m);
            var contribuyente = _context.Contribuyentes.First(c => c.RncCedula == "123456789");
            nuevoComprobante.ContribuyenteId = contribuyente.Id;

            // Act
            await _repository.AddAsync(nuevoComprobante);
            await _context.SaveChangesAsync();

            // Assert
            var comprobantesFiscales = await _repository.GetByRncCedulaAsync("123456789");
            comprobantesFiscales.Should().HaveCount(2);
            comprobantesFiscales.Should().Contain(c => c.NCF == "E310000000004");
        }

        [Fact]
        public async Task CountAsync_DeberiaRetornarCantidadCorrecta()
        {
            // Act
            var count = await _repository.CountAsync();

            // Assert
            count.Should().Be(3);
        }

        [Fact]
        public async Task GetAllAsync_DeberiaRetornarTodosLosComprobantesFiscales()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
