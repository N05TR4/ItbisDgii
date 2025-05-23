using FluentAssertions;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Exceptions;

namespace ItbisDgii.Tests.Domain.Entities
{
    public class ComprobanteFiscalTests
    {
        [Fact]
        public void Constructor_ConParametrosValidos_DeberiaCrearComprobanteFiscal()
        {
            // Arrange
            var rncCedula = "12345678901";
            var ncf = "E310000000001";
            var monto = 100m;

            // Act
            var comprobante = new ComprobanteFiscal(rncCedula, ncf, monto);

            // Assert
            comprobante.RncCedula.Should().Be(rncCedula);
            comprobante.NCF.Should().Be(ncf);
            comprobante.Monto.Should().Be(monto);
            comprobante.Itbis18.Should().Be(18m); // 18% de 100
            comprobante.Id.Should().NotBeEmpty();
            comprobante.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_ConRncCedulaInvalido_DeberiaLanzarDomainException(string rncCedulaInvalido)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new ComprobanteFiscal(rncCedulaInvalido, "E310000000001", 100m));

            exception.Message.Should().Be("RNC/Cédula no puede ser vacío");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_ConNCFInvalido_DeberiaLanzarDomainException(string ncfInvalido)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new ComprobanteFiscal("12345678901", ncfInvalido, 100m));

            exception.Message.Should().Be("NCF no puede ser vacío");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_ConMontoInvalido_DeberiaLanzarDomainException(decimal montoInvalido)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new ComprobanteFiscal("12345678901", "E310000000001", montoInvalido));

            exception.Message.Should().Be("Monto debe ser mayor que cero");
        }

        [Theory]
        [InlineData(100, 18)]
        [InlineData(500, 90)]
        [InlineData(1000, 180)]
        [InlineData(55.55, 10)]
        public void Constructor_ConMontoValido_DeberiaCalcularITBISCorrectamente(decimal monto, decimal itbisEsperado)
        {
            // Act
            var comprobante = new ComprobanteFiscal("12345678901", "E310000000001", monto);

            // Assert
            comprobante.Itbis18.Should().Be(itbisEsperado);
        }
    }
}
