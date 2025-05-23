using AutoMapper;
using FluentAssertions;
using ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Mapping;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace ItbisDgii.Tests.Application.Feaatures.ComprobanteFiscal.Commands
{
    public class CreateComprobanteFiscalCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContribuyenteRepository> _mockContribuyenteRepository;
        private readonly Mock<IComprobanteFiscalRepository> _mockComprobanteFiscalRepository;
        private readonly Mock<ILogger<CreateComprobanteFiscalCommandHandler>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly CreateComprobanteFiscalCommandHandler _handler;

        public CreateComprobanteFiscalCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContribuyenteRepository = new Mock<IContribuyenteRepository>();
            _mockComprobanteFiscalRepository = new Mock<IComprobanteFiscalRepository>();
            _mockLogger = new Mock<ILogger<CreateComprobanteFiscalCommandHandler>>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();

            _mockUnitOfWork.Setup(x => x.ContribuyenteRepository).Returns(_mockContribuyenteRepository.Object);
            _mockUnitOfWork.Setup(x => x.ComprobanteFiscalRepository).Returns(_mockComprobanteFiscalRepository.Object);

            _handler = new CreateComprobanteFiscalCommandHandler(_mockUnitOfWork.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ConContribuyenteActivoYComprobanteValido_DeberiaCrearComprobante()
        {
            // Arrange
            var command = new CreateComprobanteFiscalCommand
            {
                RncCedula = "12345678901",
                NCF = "E310000000001",
                Monto = 100m
            };

            var contribuyente = new Contribuyente(command.RncCedula, "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contribuyente);

            _mockComprobanteFiscalRepository
                .Setup(x => x.AddAsync(It.IsAny<ItbisDgii.Domain.Entities.ComprobanteFiscal>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ItbisDgii.Domain.Entities.ComprobanteFiscal c, CancellationToken ct) => c);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.RncCedula.Should().Be(command.RncCedula);
            result.NCF.Should().Be(command.NCF);
            result.Monto.Should().Be(command.Monto);
            result.Itbis18.Should().Be(18m); // 18% de 100

            _mockComprobanteFiscalRepository.Verify(x => x.AddAsync(It.IsAny<ItbisDgii.Domain.Entities.ComprobanteFiscal>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ConContribuyenteInexistente_DeberiaLanzarExcepcion()
        {
            // Arrange
            var command = new CreateComprobanteFiscalCommand
            {
                RncCedula = "12345678901",
                NCF = "E310000000001",
                Monto = 100m
            };

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contribuyente?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("no encontrado");

            _mockComprobanteFiscalRepository.Verify(x => x.AddAsync(It.IsAny<ItbisDgii.Domain.Entities.ComprobanteFiscal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ConContribuyenteInactivo_DeberiaLanzarApiException()
        {
            // Arrange
            var command = new CreateComprobanteFiscalCommand
            {
                RncCedula = "12345678901",
                NCF = "E310000000001",
                Monto = 100m
            };

            var contribuyenteInactivo = new Contribuyente(command.RncCedula, "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Inactivo);

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contribuyenteInactivo);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ItbisDgii.Application.Exceptions.ApiExceptions>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("inactivo");

            _mockComprobanteFiscalRepository.Verify(x => x.AddAsync(It.IsAny<ItbisDgii.Domain.Entities.ComprobanteFiscal>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
