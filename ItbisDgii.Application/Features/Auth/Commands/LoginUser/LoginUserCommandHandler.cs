using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Response<AuthResponseDto>>
    {

        private readonly ILogger<LoginUserCommandHandler> _logger;
        private readonly IAuthServices _authServices;

        public LoginUserCommandHandler(ILogger<LoginUserCommandHandler> logger,  IAuthServices authServices)
        {

            _logger = logger;
            _authServices = authServices ?? throw new ArgumentNullException(nameof(authServices));
        }

        public async Task<Response<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await _authServices.Authenticate(new AuthRequestDto
            {
                Email = request.Email,
                Password = request.Password
            }, request.IpAddress);
        }
    }
}
