using AutoMapper;
using FluentAssertions;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Mapping;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ItbisDgii.Tests.Application.Features.Contribuyentes.Commands
{
    public class CreateContribuyenteCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContribuyenteRepository> _mockContribuyenteRepository;
        private readonly Mock<ILogger<CreateContribuyenteCommandHandler>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly CreateContribuyenteCommandHandler _handler;

        public CreateContribuyenteCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContribuyenteRepository = new Mock<IContribuyenteRepository>();
            _mockLogger = new Mock<ILogger<CreateContribuyenteCommandHandler>>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();

            _mockUnitOfWork.Setup(x => x.ContribuyenteRepository).Returns(_mockContribuyenteRepository.Object);

            _handler = new CreateContribuyenteCommandHandler(_mockUnitOfWork.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ConContribuyenteValido_DeberiaCrearContribuyente()
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contribuyente?)null);

            _mockContribuyenteRepository
                .Setup(x => x.AddAsync(It.IsAny<Contribuyente>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contribuyente c, CancellationToken ct) => c);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.RncCedula.Should().Be(command.RncCedula);
            result.Data.Nombre.Should().Be(command.Nombre);
            result.Data.Tipo.Should().Be(command.Tipo);
            result.Data.Estatus.Should().Be(command.Estatus);

            _mockContribuyenteRepository.Verify(x => x.AddAsync(It.IsAny<Contribuyente>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ConContribuyenteExistente_DeberiaRetornarError()
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = "PersonaFisica",
                Estatus = "Activo"
            };

            var contribuyenteExistente = new Contribuyente(command.RncCedula, command.Nombre, TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contribuyenteExistente);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("ya existe");

            _mockContribuyenteRepository.Verify(x => x.AddAsync(It.IsAny<Contribuyente>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("TipoInvalido")]
        [InlineData("")]
        public async Task Handle_ConTipoInvalido_DeberiaRetornarError(string tipoInvalido)
        {
            // Arrange
            var command = new CreateContribuyenteCommand
            {
                RncCedula = "12345678901",
                Nombre = "Juan Pérez",
                Tipo = tipoInvalido,
                Estatus = "Activo"
            };

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(command.RncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contribuyente?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("inválido");

            _mockContribuyenteRepository.Verify(x => x.AddAsync(It.IsAny<Contribuyente>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}