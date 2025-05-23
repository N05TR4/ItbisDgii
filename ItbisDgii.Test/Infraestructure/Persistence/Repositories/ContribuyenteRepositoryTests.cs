using FluentAssertions;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Persistence.Context;
using ItbisDgii.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ItbisDgii.Tests.Infrastructure.Persistence.Repositories
{
    public class ContribuyenteRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ContribuyenteRepository _repository;

        public ContribuyenteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ContribuyenteRepository(_context);

            // Seed initial data
            SeedData();
        }

        private void SeedData()
        {
            var contribuyente1 = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);
            var contribuyente2 = new Contribuyente("123456789", "Farmacia ABC", TipoContribuyente.PersonaJuridica, EstatusContribuyente.Activo);

            var comprobante1 = new ComprobanteFiscal("12345678901", "E310000000001", 100m);
            var comprobante2 = new ComprobanteFiscal("12345678901", "E310000000002", 200m);

            contribuyente1.ComprobantesFiscales.Add(comprobante1);
            contribuyente1.ComprobantesFiscales.Add(comprobante2);

            _context.Contribuyentes.AddRange(contribuyente1, contribuyente2);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByRncCedulaAsync_ConRncExistente_DeberiaRetornarContribuyente()
        {
            // Act
            var result = await _repository.GetByRncCedulaAsync("12345678901");

            // Assert
            result.Should().NotBeNull();
            result!.RncCedula.Should().Be("12345678901");
            result.Nombre.Should().Be("Juan Pérez");
            result.ComprobantesFiscales.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByRncCedulaAsync_ConRncInexistente_DeberiaRetornarNull()
        {
            // Act
            var result = await _repository.GetByRncCedulaAsync("99999999999");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllWithComprobantesAsync_DeberiaRetornarTodosLosContribuyentesConComprobantesCargados()
        {
            // Act
            var result = await _repository.GetAllWithComprobantesAsync();

            // Assert
            result.Should().HaveCount(2);
            var contribuyenteConComprobantesFiscales = result.First(c => c.RncCedula == "12345678901");
            contribuyenteConComprobantesFiscales.ComprobantesFiscales.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddAsync_ConNuevoContribuyente_DeberiaAgregarCorrectamente()
        {
            // Arrange
            var nuevoContribuyente = new Contribuyente("11111111111", "María García", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            // Act
            await _repository.AddAsync(nuevoContribuyente);
            await _context.SaveChangesAsync();

            // Assert
            var contribuyenteGuardado = await _repository.GetByRncCedulaAsync("11111111111");
            contribuyenteGuardado.Should().NotBeNull();
            contribuyenteGuardado!.Nombre.Should().Be("María García");
        }

        [Fact]
        public async Task CountAsync_DeberiaRetornarCantidadCorrecta()
        {
            // Act
            var count = await _repository.CountAsync();

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateAsync_ConContribuyenteModificado_DeberiaActualizar()
        {
            // Arrange
            var contribuyente = await _repository.GetByRncCedulaAsync("12345678901");
            contribuyente!.ActualizarEstatus(EstatusContribuyente.Inactivo);

            // Act
            await _repository.UpdateAsync(contribuyente);
            await _context.SaveChangesAsync();

            // Assert
            var contribuyenteActualizado = await _repository.GetByRncCedulaAsync("12345678901");
            contribuyenteActualizado!.Estatus.Should().Be(EstatusContribuyente.Inactivo);
        }

        [Fact]
        public async Task DeleteAsync_ConContribuyenteExistente_DeberiaEliminar()
        {
            // Arrange
            var contribuyente = await _repository.GetByRncCedulaAsync("123456789");

            // Act
            await _repository.DeleteAsync(contribuyente!);
            await _context.SaveChangesAsync();

            // Assert
            var contribuyenteEliminado = await _repository.GetByRncCedulaAsync("123456789");
            contribuyenteEliminado.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}