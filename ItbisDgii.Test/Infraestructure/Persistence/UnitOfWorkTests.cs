using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Infraestructure.Persistence.Context;
using ItbisDgii.Infraestructure.Persistence.Repositories;
using ItbisDgii.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace ItbisDgii.Tests.Infraestructure.Persistence
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Fact]
        public void ContribuyenteRepository_DeberiaRetornarInstancia()
        {
            // Act
            var repository = _unitOfWork.ContribuyenteRepository;

            // Assert
            repository.Should().NotBeNull();
            repository.Should().BeOfType<ContribuyenteRepository>();
        }

        [Fact]
        public void ComprobanteFiscalRepository_DeberiaRetornarInstancia()
        {
            // Act
            var repository = _unitOfWork.ComprobanteFiscalRepository;

            // Assert
            repository.Should().NotBeNull();
            repository.Should().BeOfType<ComprobanteFiscalRepository>();
        }

        [Fact]
        public void ContribuyenteRepository_VariasLlamadas_DeberiaRetornarMismaInstancia()
        {
            // Act
            var repository1 = _unitOfWork.ContribuyenteRepository;
            var repository2 = _unitOfWork.ContribuyenteRepository;

            // Assert
            repository1.Should().BeSameAs(repository2);
        }

        [Fact]
        public async Task SaveChangesAsync_ConCambios_DeberiaRetornarCantidadAfectada()
        {
            // Arrange
            var contribuyente = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);
            await _unitOfWork.ContribuyenteRepository.AddAsync(contribuyente);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task SaveChangesAsync_SinCambios_DeberiaRetornarCero()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(0);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _context.Dispose();
        }
    }
}
