using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Features.Auth.Commands.LoginUser;
using ItbisDgii.Application.Features.Auth.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace ItbisDgii.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync(AuthRequestDto request)
        {
            return Ok(await Mediator.Send(new LoginUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GenerateIpAdress()
            }));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            return Ok(await Mediator.Send(new RegisterCommand
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                Origin = Request.Headers["origin"]
            }));
        }

        private string GenerateIpAdress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}
