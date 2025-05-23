using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<Response<AuthResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }
}
