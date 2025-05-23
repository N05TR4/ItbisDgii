using FluentAssertions;
using ItbisDgii.API.Controllers;
using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Features.Auth.Commands.LoginUser;
using ItbisDgii.Application.Features.Auth.Commands.RegisterUser;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;

namespace ItbisDgii.Tests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();

            // Crear servicios mock
            var services = new ServiceCollection();
            services.AddSingleton(_mockMediator.Object);
            var serviceProvider = services.BuildServiceProvider();

            // Crear contexto HTTP mock
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider,
                Connection = { RemoteIpAddress = IPAddress.Parse("192.168.1.1") }
            };

            httpContext.Request.Headers["origin"] = "http://localhost:3000";

            // Crear controlador y configurar contexto
            _controller = new AuthController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task AuthenticateAsync_ConCredencialesValidas_DeberiaRetornarOk()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var authResponse = new AuthResponseDto
            {
                Id = "user-id",
                Email = request.Email,
                UserName = "testuser",
                JWToken = "jwt-token",
                Roles = new List<string> { "Basic" },
                IsVerified = true
            };

            var response = new Response<AuthResponseDto>(authResponse, "Login exitoso");

            _mockMediator
                .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AuthenticateAsync(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);

            // Verificar que se envió el comando correcto
            _mockMediator.Verify(x => x.Send(
                It.Is<LoginUserCommand>(cmd =>
                    cmd.Email == request.Email &&
                    cmd.Password == request.Password &&
                    !string.IsNullOrEmpty(cmd.IpAddress)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ConDatosValidos_DeberiaRetornarOk()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan.perez@test.com",
                UserName = "jperez",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var response = new Response<string>("user-id", "Usuario registrado exitosamente");

            _mockMediator
                .Setup(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.RegisterAsync(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);

            // Verificar que se envió el comando correcto
            _mockMediator.Verify(x => x.Send(
                It.Is<RegisterCommand>(cmd =>
                    cmd.Email == request.Email &&
                    cmd.UserName == request.UserName &&
                    cmd.Origin == "http://localhost:3000"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ConCredencialesInvalidas_DeberiaRetornarBadRequest()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            _mockMediator
                .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationException("Credenciales incorrectas"));

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _controller.AuthenticateAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ConEmailDuplicado_DeberiaLanzarExcepcion()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "existing@test.com",
                UserName = "jperez",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            _mockMediator
                .Setup(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationException("El Email ya existe"));

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _controller.RegisterAsync(request));
        }

        [Fact]
        public async Task AuthenticateAsync_DeberiaGenerarIpAddressCorrectamente()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var response = new Response<AuthResponseDto>(new AuthResponseDto(), "Login exitoso");

            _mockMediator
                .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            await _controller.AuthenticateAsync(request);

            // Assert - Verificar que se generó IP address
            _mockMediator.Verify(x => x.Send(
                It.Is<LoginUserCommand>(cmd => cmd.IpAddress == "192.168.1.1"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ConXForwardedFor_DeberiaUsarHeaderIP()
        {
            // Arrange
            var forwardedIP = "203.0.113.1";
            _controller.HttpContext.Request.Headers["X-Forwarded-For"] = forwardedIP;

            var request = new AuthRequestDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var response = new Response<AuthResponseDto>(new AuthResponseDto(), "Login exitoso");

            _mockMediator
                .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            await _controller.AuthenticateAsync(request);

            // Assert - Verificar que se usó la IP del header
            _mockMediator.Verify(x => x.Send(
                It.Is<LoginUserCommand>(cmd => cmd.IpAddress == forwardedIP),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_DeberiaCapturarOriginCorrectamente()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan.perez@test.com",
                UserName = "jperez",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var response = new Response<string>("user-id", "Usuario registrado exitosamente");

            _mockMediator
                .Setup(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            await _controller.RegisterAsync(request);

            // Assert - Verificar que se capturó el origin
            _mockMediator.Verify(x => x.Send(
                It.Is<RegisterCommand>(cmd => cmd.Origin == "http://localhost:3000"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    
}
     
