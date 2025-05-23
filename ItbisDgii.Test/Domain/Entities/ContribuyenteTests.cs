using FluentAssertions;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Domain.Exceptions;

namespace ItbisDgii.Tests.Domain.Entities
{
    public class ContribuyenteTests
    {
        [Fact]
        public void Constructor_ConParametrosValidos_DeberiaCrearContribuyente()
        {
            // Arrange
            var rncCedula = "12345678901";
            var nombre = "Juan Pérez";
            var tipo = TipoContribuyente.PersonaFisica;
            var estatus = EstatusContribuyente.Activo;

            // Act
            var contribuyente = new Contribuyente(rncCedula, nombre, tipo, estatus);

            // Assert
            contribuyente.RncCedula.Should().Be(rncCedula);
            contribuyente.Nombre.Should().Be(nombre);
            contribuyente.Tipo.Should().Be(tipo);
            contribuyente.Estatus.Should().Be(estatus);
            contribuyente.Id.Should().NotBeEmpty();
            contribuyente.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_ConRncCedulaInvalido_DeberiaLanzarDomainException(string rncCedulaInvalido)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new Contribuyente(rncCedulaInvalido, "Nombre", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo));

            exception.Message.Should().Be("RNC/Cédula no puede ser vacío");
        }

        [Fact]
        public void Constructor_PersonaFisicaConRncIncorrecto_DeberiaLanzarDomainException()
        {
            // Arrange
            var rncInvalido = "123456789"; // 9 dígitos para persona física

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new Contribuyente(rncInvalido, "Nombre", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo));

            exception.Message.Should().Be("RNC/Cédula para persona física debe tener 11 dígitos");
        }

        [Fact]
        public void Constructor_PersonaJuridicaConRncIncorrecto_DeberiaLanzarDomainException()
        {
            // Arrange
            var rncInvalido = "12345678901"; // 11 dígitos para persona jurídica

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new Contribuyente(rncInvalido, "Nombre", TipoContribuyente.PersonaJuridica, EstatusContribuyente.Activo));

            exception.Message.Should().Be("RNC para persona jurídica debe tener 9 dígitos");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_ConNombreInvalido_DeberiaLanzarDomainException(string nombreInvalido)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                new Contribuyente("12345678901", nombreInvalido, TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo));

            exception.Message.Should().Be("Nombre no puede ser vacío");
        }

        [Fact]
        public void ActualizarEstatus_ConNuevoEstatus_DeberiaActualizarEstatus()
        {
            // Arrange
            var contribuyente = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            // Act
            contribuyente.ActualizarEstatus(EstatusContribuyente.Inactivo);

            // Assert
            contribuyente.Estatus.Should().Be(EstatusContribuyente.Inactivo);
        }

        [Fact]
        public void CalcularTotalITBIS_SinComprobantesFiscales_DeberiaRetornarCero()
        {
            // Arrange
            var contribuyente = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            // Act
            var total = contribuyente.CalcularTotalITBIS();

            // Assert
            total.Should().Be(0);
        }

        [Fact]
        public void CalcularTotalITBIS_ConComprobantesFiscales_DeberiaRetornarSuma()
        {
            // Arrange
            var contribuyente = new Contribuyente("12345678901", "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            var comprobante1 = new ComprobanteFiscal("12345678901", "E310000000001", 100m);
            var comprobante2 = new ComprobanteFiscal("12345678901", "E310000000002", 200m);

            contribuyente.ComprobantesFiscales.Add(comprobante1);
            contribuyente.ComprobantesFiscales.Add(comprobante2);

            // Act
            var total = contribuyente.CalcularTotalITBIS();

            // Assert
            total.Should().Be(54m); // 18 + 36 = 54
        }
    }
}
