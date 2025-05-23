using FluentAssertions;
using FluentValidation.TestHelper;
using ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente;
using Xunit;

namespace ItbisDgii.Tests.Application.Features.Contribuyentes.Commands
{
    public class CreateContribuyenteCommandValidatorTests
    {
        private readonly CreateContribuyenteCommandValidator _validator;

        public CreateContribuyenteCommandValidatorTests()
        {
            _validator = new CreateContribuyenteCommandValidator();
        }

        [Fact]
        public void Validate_ConComandoValido_NoDeberiaFallar()
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("PersonaJuridica", "123456789")] // 9 dígitos para persona jurídica - Válido
        [InlineData("PersonaFisica", "12345678901")] // 11 dígitos para persona física - Válido
        public void Validate_ConRncCedulaValido_NoDeberiaFallar(string tipo, string rncCedula)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedula,
                Nombre = "Juan Pérez",
                Tipo = tipo,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_ConRncCedulaVacio_DeberiaFallar(string rncCedulaInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedulaInvalido,
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RncCedula)
                .WithErrorMessage("RNC/Cédula es requerido");
        }

        [Theory]
        [InlineData("PersonaFisica", "1234567890")] // 10 dígitos para persona física
        [InlineData("PersonaFisica", "123456789012")] // 12 dígitos para persona física
        [InlineData("PersonaJuridica", "12345678")] // 8 dígitos para persona jurídica
        [InlineData("PersonaJuridica", "1234567890")] // 10 dígitos para persona jurídica
        public void Validate_ConRncCedulaLongitudIncorrecta_DeberiaFallar(string tipo, string rncCedula)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedula,
                Nombre = "Juan Pérez",
                Tipo = tipo,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RncCedula)
                .WithErrorMessage("RNC/Cédula debe tener 9 dígitos para persona jurídica o 11 dígitos para persona física");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_ConNombreVacio_DeberiaFallar(string nombreInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = nombreInvalido,
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nombre)
                .WithErrorMessage("Nombre es requerido");
        }

        [Fact]
        public void Validate_ConNombreMuyLargo_DeberiaFallar()
        {
            // Arrange
            var nombreLargo = new string('A', 101); // 101 caracteres
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = nombreLargo,
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nombre)
                .WithErrorMessage("Nombre no puede exceder 100 caracteres");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_ConTipoVacio_DeberiaFallar(string tipoInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = tipoInvalido,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tipo)
                .WithErrorMessage("Tipo es requerido");
        }

        [Theory]
        [InlineData("TipoInvalido")]
        [InlineData("PersonaFísica")] // Con tilde
        [InlineData("PersonaJurídica")] // Con tilde
        [InlineData("Fisica")]
        [InlineData("Juridica")]
        [InlineData("Physical")] // En inglés
        [InlineData("Legal")] // En inglés
        [InlineData("PF")] // Abreviación
        [InlineData("PJ")] // Abreviación
        [InlineData("123")] // Números
        public void Validate_ConTipoInvalido_DeberiaFallar(string tipoInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = tipoInvalido,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tipo)
                .WithErrorMessage("Tipo debe ser 'PersonaFisica' o 'PersonaJuridica'");
        }

        [Theory]
        [InlineData("PersonaFisica")]
        [InlineData("PersonaJuridica")]
        [InlineData("personafisica")] // Case insensitive
        [InlineData("personajuridica")] // Case insensitive
        [InlineData("PERSONAFISICA")] // Case insensitive
        [InlineData("PERSONAJURIDICA")] // Case insensitive
        [InlineData("PersonaFIsica")] // Mixed case
        [InlineData("PersonaJuRidica")] // Mixed case
        public void Validate_ConTipoValido_NoDeberiaFallar(string tipoValido)
        {
            // Arrange
            var rncCedula = tipoValido.ToLower().Contains("fisica") ? "12345678901" : "123456789";
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedula,
                Nombre = "Juan Pérez",
                Tipo = tipoValido,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Tipo);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_ConEstatusVacio_DeberiaFallar(string estatusInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = estatusInvalido
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Estatus)
                .WithErrorMessage("Estatus es requerido");
        }

        [Theory]
        [InlineData("EstatusInvalido")]
        [InlineData("Active")] // En inglés
        [InlineData("Inactive")] // En inglés
        [InlineData("Pendiente")] // Otro estatus
        [InlineData("Suspendido")] // Otro estatus
        [InlineData("123")] // Números
        [InlineData("ACT")] // Abreviación
        public void Validate_ConEstatusInvalido_DeberiaFallar(string estatusInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = estatusInvalido
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Estatus)
                .WithErrorMessage("Estatus debe ser 'Activo' o 'Inactivo'");
        }

        [Theory]
        [InlineData("Activo")]
        [InlineData("Inactivo")]
        [InlineData("activo")] // Case insensitive
        [InlineData("inactivo")] // Case insensitive
        [InlineData("ACTIVO")] // Case insensitive
        [InlineData("INACTIVO")] // Case insensitive
        [InlineData("AcTiVo")] // Mixed case
        [InlineData("InAcTiVo")] // Mixed case
        public void Validate_ConEstatusValido_NoDeberiaFallar(string estatusValido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = estatusValido
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Estatus);
        }

        [Fact]
        public void Validate_ConTodosLosCamposInvalidos_DeberiaFallarTodos()
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "", // Vacío
                Nombre = null, // Null
                Tipo = "TipoInvalido", // Inválido
                Estatus = "EstatusInvalido" // Inválido
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RncCedula);
            result.ShouldHaveValidationErrorFor(x => x.Nombre);
            result.ShouldHaveValidationErrorFor(x => x.Tipo);
            result.ShouldHaveValidationErrorFor(x => x.Estatus);
        }

        [Theory]
        [InlineData("PersonaFisica", "12345678901", "Juan Pérez", "Activo")]
        [InlineData("PersonaJuridica", "123456789", "Farmacia ABC SRL", "Inactivo")]
        [InlineData("personafisica", "98765432101", "María García", "activo")] // Case insensitive
        [InlineData("PERSONAJURIDICA", "987654321", "EMPRESA XYZ", "INACTIVO")] // Case insensitive
        public void Validate_ConCombinacionesValidas_NoDeberiaFallar(string tipo, string rncCedula, string nombre, string estatus)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedula,
                Nombre = nombre,
                Tipo = tipo,
                Estatus = estatus
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ConNombreEnLimiteMaximo_NoDeberiaFallar()
        {
            // Arrange
            var nombreLimite = new string('A', 100); // Exactamente 100 caracteres
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = nombreLimite,
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Nombre);
        }

        [Theory]
        [InlineData("PersonaFisica", null)] // Tipo válido pero RNC null
        [InlineData("PersonaJuridica", "")] // Tipo válido pero RNC vacío
        [InlineData(null, "12345678901")] // RNC válido pero tipo null
        [InlineData("", "123456789")] // RNC válido pero tipo vacío
        public void Validate_ConDependenciasInvalidas_DeberiaFallar(string tipo, string rncCedula)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = rncCedula,
                Nombre = "Juan Pérez",
                Tipo = tipo,
                Estatus = "Activo"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            // Debería fallar en RNC/Cédula o Tipo
            result.Errors.Should().NotBeEmpty();
        }
    }
}