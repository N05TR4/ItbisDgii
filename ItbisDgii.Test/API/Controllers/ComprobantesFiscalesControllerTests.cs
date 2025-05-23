using FluentAssertions;
using ItbisDgii.API.Controllers;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetAllComprobantesFiscales;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetComprobantesFiscalesByRncCedula;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetTotalITBISByRncCedula;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ItbisDgii.Tests.API.Controllers
{
    public class ComprobantesFiscalesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ComprobantesFiscalesController>> _mockLogger;
        private readonly ComprobantesFiscalesController _controller;

        public ComprobantesFiscalesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ComprobantesFiscalesController>>();
            _controller = new ComprobantesFiscalesController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_DeberiaRetornarOkConLista()
        {
            // Arrange
            var comprobantesFiscales = new List<ComprobanteFiscalDto>
            {
                new ComprobanteFiscalDto
                {
                    Id = Guid.NewGuid(),
                    RncCedula = "12345678901",
                    NCF = "E310000000001",
                    Monto = 100m,
                    Itbis18 = 18m
                }
            };

            var paginatedResponse = new PaginatedResponse<ComprobanteFiscalDto>
            {
                Items = comprobantesFiscales,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetAllComprobantesFiscalesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var actionResult = await _controller.GetAll();

            // Assert
            var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(paginatedResponse);
        }

        [Fact]
        public async Task GetByContribuyente_ConRncExistente_DeberiaRetornarOk()
        {
            // Arrange
            var rncCedula = "12345678901";
            var comprobantesFiscales = new List<ComprobanteFiscalDto>
            {
                new ComprobanteFiscalDto
                {
                    Id = Guid.NewGuid(),
                    RncCedula = rncCedula,
                    NCF = "E310000000001",
                    Monto = 100m,
                    Itbis18 = 18m
                }
            };

            var response = new Response<IEnumerable<ComprobanteFiscalDto>>(comprobantesFiscales, "Comprobantes encontrados");

            _mockMediator
                .Setup(x => x.Send(It.Is<GetComprobantesFiscalesByRncCedulaQuery>(q => q.RncCedula == rncCedula), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await _controller.GetByContribuyente(rncCedula);

            // Assert
            var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetTotalITBIS_ConRncExistente_DeberiaRetornarOk()
        {
            // Arrange
            var rncCedula = "12345678901";
            var totalITBIS = 54m;

            _mockMediator
                .Setup(x => x.Send(It.Is<GetTotalITBISByRncCedulaQuery>(q => q.RncCedula == rncCedula), It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalITBIS);

            // Act
            var actionResult = await _controller.GetTotalITBIS(rncCedula);

            // Assert
            var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(totalITBIS);
        }

        [Fact]
        public async Task Create_ConComandoValido_DeberiaRetornarCreatedAtAction()
        {
            // Arrange
            var command = new CreateComprobanteFiscalCommand
            {
                RncCedula = "12345678901",
                NCF = "E310000000001",
                Monto = 100m
            };

            var comprobanteFiscalDto = new ComprobanteFiscalDto
            {
                Id = Guid.NewGuid(),
                RncCedula = command.RncCedula,
                NCF = command.NCF,
                Monto = command.Monto,
                Itbis18 = 18m
            };

            _mockMediator
                .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(comprobanteFiscalDto);

            // Act
            var actionResult = await _controller.Create(command);

            // Assert
            var result = actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            result.ActionName.Should().Be(nameof(_controller.GetByContribuyente));
            result.RouteValues.Should().ContainKey("rncCedula");
            result.RouteValues["rncCedula"].Should().Be(command.RncCedula);
            result.Value.Should().Be(comprobanteFiscalDto);
        }
    }
}

