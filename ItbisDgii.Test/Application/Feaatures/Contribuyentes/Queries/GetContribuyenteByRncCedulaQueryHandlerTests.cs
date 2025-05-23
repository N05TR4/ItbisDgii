using AutoMapper;
using FluentAssertions;
using ItbisDgii.Application.Features.Contribuyentes.Queries.GetContribuyenteByRncCedula;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Mapping;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace ItbisDgii.Tests.Application.Feaatures.Contribuyentes.Queries
{
    public class GetContribuyenteByRncCedulaQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContribuyenteRepository> _mockContribuyenteRepository;
        private readonly Mock<ILogger<GetContribuyenteByRncCedulaQueryHandler>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly GetContribuyenteByRncCedulaQueryHandler _handler;

        public GetContribuyenteByRncCedulaQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContribuyenteRepository = new Mock<IContribuyenteRepository>();
            _mockLogger = new Mock<ILogger<GetContribuyenteByRncCedulaQueryHandler>>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();

            _mockUnitOfWork.Setup(x => x.ContribuyenteRepository).Returns(_mockContribuyenteRepository.Object);

            _handler = new GetContribuyenteByRncCedulaQueryHandler(_mockUnitOfWork.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ConContribuyenteExistente_DeberiaRetornarContribuyente()
        {
            // Arrange
            var rncCedula = "12345678901";
            var query = new GetContribuyenteByRncCedulaQuery { RncCedula = rncCedula };

            var contribuyente = new Contribuyente(rncCedula, "Juan Pérez", TipoContribuyente.PersonaFisica, EstatusContribuyente.Activo);

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(rncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contribuyente);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.RncCedula.Should().Be(rncCedula);
            result.Data.Nombre.Should().Be("Juan Pérez");
        }

        [Fact]
        public async Task Handle_ConContribuyenteInexistente_DeberiaRetornarError()
        {
            // Arrange
            var rncCedula = "12345678901";
            var query = new GetContribuyenteByRncCedulaQuery { RncCedula = rncCedula };

            _mockContribuyenteRepository
                .Setup(x => x.GetByRncCedulaAsync(rncCedula, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contribuyente?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }
    }
}
