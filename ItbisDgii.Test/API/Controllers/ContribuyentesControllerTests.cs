using FluentAssertions;
using ItbisDgii.API.Controllers;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente;
using ItbisDgii.Application.Features.Contribuyentes.Queries.GetAllContribuyentes;
using ItbisDgii.Application.Features.Contribuyentes.Queries.GetContribuyenteByRncCedula;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ItbisDgii.Tests.API.Controllers
{
    public class ContribuyentesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ContribuyentesController>> _mockLogger;
        private readonly ContribuyentesController _controller;

        public ContribuyentesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ContribuyentesController>>();
            _controller = new ContribuyentesController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_DeberiaRetornarOkConLista()
        {
            // Arrange
            var contribuyentes = new List<ContribuyenteDto>
            {
                new ContribuyenteDto
                {
                    Id = Guid.NewGuid(),
                    RncCedula = "12345678901",
                    Nombre = "Juan Pérez",
                    Tipo = "PersonaFisica",
                    Estatus = "Activo",
                    TotalITBIS = 54m
                }
            };

            var paginatedResponse = new PaginatedResponse<ContribuyenteDto>
            {
                Items = contribuyentes,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetAllContribuyentesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var actionResult = await _controller.GetAll();

            // Assert
            var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(paginatedResponse);
        }

        [Fact]
        public async Task GetByRncCedula_ConRncExistente_DeberiaRetornarOk()
        {
            // Arrange
            var rncCedula = "12345678901";
            var contribuyenteDto = new ContribuyenteDto
            {
                Id = Guid.NewGuid(),
                RncCedula = rncCedula,
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo",
                TotalITBIS = 54m
            };

            var response = new Response<ContribuyenteDto>(contribuyenteDto, "Contribuyente encontrado");

            _mockMediator
                .Setup(x => x.Send(It.Is<GetContribuyenteByRncCedulaQuery>(q => q.RncCedula == rncCedula), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await _controller.GetByRncCedula(rncCedula);

            // Assert
            var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task Create_ConComandoValido_DeberiaRetornarCreatedAtAction()
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            var contribuyenteDto = new ContribuyenteDto
            {
                Id = Guid.NewGuid(),
                RncCedula = command.RncCedula,
                Nombre = command.Nombre,
                Tipo = command.Tipo,
                Estatus = command.Estatus,
                TotalITBIS = 0m
            };

            var response = new Response<ContribuyenteDto>(contribuyenteDto, "Contribuyente creado");

            _mockMediator
                .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await _controller.Create(command);

            // Assert
            var result = actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            result.ActionName.Should().Be(nameof(_controller.GetByRncCedula));
            result.RouteValues.Should().ContainKey("rncCedula");
            result.RouteValues["rncCedula"].Should().Be(command.RncCedula);
            result.Value.Should().Be(response);
        }

        [Fact]
        public void Controller_DeberiaLoguearCorrectamente()
        {
            // Arrange & Act
            var result = _controller.GetAll();

            // Assert - Verificar que se llamó al logger
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Getting all contribuyentes")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}